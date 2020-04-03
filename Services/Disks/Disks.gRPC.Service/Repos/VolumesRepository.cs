using Disks.gRPC.Service.Data;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Repos
{
    public class VolumesRepository : IVolumeDataSource
    {
        /// <summary>
        /// The service which will provide a connection to the service
        /// </summary>
        private readonly RedisService redisService;

        private readonly ILogger<VolumesRepository> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="redisService">Redis connection provider</param>
        /// <param name="logger">Logger</param>
        public VolumesRepository(RedisService redisService, ILogger<VolumesRepository> logger)
        {
            this.redisService = redisService;
            this.logger = logger;
        }

        /// <summary>
        /// Create a volume
        /// </summary>
        /// <param name="createVolumeRequest">Parameters for create</param>
        /// <exception cref="VolumeException">If cannot create a volume</exception>
        /// <returns>gRPC reply with volume id</returns>
        public async Task<VolumeReply> Create(CreateVolumeRequest createVolumeRequest)
        {
            logger.LogInformation("Requested for creaton of a volume"); // english 

            string volumeId = EntityIdGenerator.Create();
            VolumeModel volume = VolumeAdapter.Volume(volumeId, createVolumeRequest);

            logger.LogInformation($"Created the VolumeModel with Id {volumeId}");

            using ConnectionMultiplexer redis = redisService.Connect();
            IDatabase db = redis.GetDatabase();

            logger.LogInformation($"Connected to the Redis servier");

            ITransaction transaction = db.CreateTransaction();
            logger.LogInformation($"Bigin transaction");

            foreach (PropertyInfo propInfo in volume.GetType().GetProperties())
            {
                _ = transaction.HashSetAsync(volumeId, propInfo.Name, propInfo.GetValue(volume).ToString());               
            }

            if (await transaction.ExecuteAsync() == false)
            {
                logger.LogError("Cannto create a volume. Transaction failed");
                throw new VolumeException("Cannot create a volume");
            }
            logger.LogInformation($"Creansaction completed successfully");
            return VolumeAdapter.Volume(volume);
        }

        /// <summary>
        /// Gets volume by id
        /// </summary>
        /// <param name="key">Volume id (vol-xxxxxxxxx)</param>
        /// <returns></returns>
        public async Task<VolumeReply> Get(string key)
        {
            logger.LogInformation($"Searching the volume with id {key}");
            logger.LogDebug($"Try to connect to the Redis");
            using ConnectionMultiplexer redis = redisService.Connect();
            var db = redis.GetDatabase();
            logger.LogDebug($"Connected to the Redis");

            if (db.KeyExists(key) == false)
            {
                logger.LogError($"Cannot find a volume with id {key}");
                throw new VolumeException($"Cannot find a volume with id {key}");
            }
            logger.LogInformation($"The volume with id {key} is exists in the store. " +
                $"Reqding the values..");

            VolumeModel volumeModel = new VolumeModel();
            foreach (PropertyInfo property in volumeModel.GetType().GetProperties())
            {
                var value = await db.HashGetAsync(key, property.Name);
                property.SetValue(volumeModel, Convert.ChangeType(value, property.PropertyType));
            }

            logger.LogInformation($"The volume successfully read from the store");
            return VolumeAdapter.Volume(volumeModel);
        }

        public async Task<IEnumerable<VolumeReply>> List()
        {
            return await Task.FromResult(new List<VolumeReply>() { new VolumeReply() }); // TODO
            
            //using ConnectionMultiplexer redis = redisService.Connect();
            //var db = redis.GetDatabase();

            //db.HashGetAll


        }
    }
}
