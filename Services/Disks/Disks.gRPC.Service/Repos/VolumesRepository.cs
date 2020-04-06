using CommonLib;
using Disks.gRPC.Service.Data;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Repos
{
    /// <summary>
    /// This is a place for volume managment e.g. saveing to DB and searching 
    /// </summary>
    public class VolumesRepository : IVolumeDataSource
    {
        /// <summary>
        /// The service which will provide a connection to the service
        /// </summary>
        private readonly RedisService redisService;

        /// <summary>
        /// Custom logger
        /// </summary>
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
        /// <param name="userId">User id</param>
        /// <exception cref="VolumeException">If cannot create a volume</exception>
        /// <returns>gRPC reply with volume id</returns>
        public async Task<VolumeReply> Create(CreateVolumeRequest createVolumeRequest, string userId)
        {
            logger.LogInformation("Requested for creaton of a volume"); // english 

            string volumeId = IdentityFabric.GenVolumeId();
            VolumeModel volume = VolumeAdapter.Volume(volumeId, createVolumeRequest);

            logger.LogInformation($"Created the VolumeModel with Id {volumeId}");

            using ConnectionMultiplexer redis = redisService.Connect();
            IDatabase db = redis.GetDatabase();

            logger.LogInformation($"Connected to the Redis servier");

            ITransaction transaction = db.CreateTransaction();
            logger.LogInformation($"Bigin transaction");

            string redisKey = GetVolumeKey(userId, volumeId);
            foreach (PropertyInfo propInfo in volume.GetType().GetProperties())
            {
                _ = transaction.HashSetAsync(redisKey, propInfo.Name, propInfo.GetValue(volume).ToString());               
            }

            if (await transaction.ExecuteAsync() == false)
            {
                logger.LogError("Cannot create a volume. Transaction failed");
                throw new VolumeException("Cannot create a volume");
            }
            logger.LogInformation($"Creansaction completed successfully");
            return VolumeAdapter.Volume(volume);
        }

        /// <summary>
        /// Gets volume by id
        /// </summary>
        /// <param name="volumeId">Volume id (vol-xxxxxxxxx)</param>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public async Task<VolumeReply> Get(string volumeId, string userId)
        {
            logger.LogInformation($"Searching the volume with id {volumeId}");
            logger.LogDebug($"Try to connect to the Redis");
            using ConnectionMultiplexer redis = redisService.Connect();
            var db = redis.GetDatabase();
            logger.LogDebug($"Connected to the Redis");

            string redisKey = GetVolumeKey(userId, volumeId);
            if (db.KeyExists(redisKey) == false)
            {
                logger.LogError($"Cannot find a volume with id {redisKey}");
                throw new VolumeException($"Cannot find a volume with id {redisKey}");
            }
            logger.LogInformation($"The volume with id {redisKey} is exists in the store. " +
                $"Reqding the values..");

            VolumeModel volumeModel = new VolumeModel();
            foreach (PropertyInfo property in volumeModel.GetType().GetProperties())
            {
                var value = await db.HashGetAsync(redisKey, property.Name);
                property.SetValue(volumeModel, Convert.ChangeType(value, property.PropertyType));
            }

            logger.LogInformation($"The volume successfully read from the store");
            return VolumeAdapter.Volume(volumeModel);
        }

        /// <summary>
        /// List of volumes by user Id
        /// </summary>
        /// <param name="userId">Use Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<VolumeReply>> List(string userId)
        {
            using ConnectionMultiplexer redis = redisService.Connect();
            var db = redis.GetDatabase();

            logger.LogDebug("Connected to Redis");

            string redisKey = GetVolumeKey(userId);
            logger.LogDebug($"Searching by key pattern: {redisKey}");

            EndPoint endPoint = redis.GetEndPoints().First() ?? throw new VolumeException($"Cannot find an endpoint");
            RedisKey[] keys = redis.GetServer(endPoint).Keys(pattern: redisKey).ToArray();
            logger.LogDebug($"Found {keys.Length} keys");
            
            List<VolumeModel> volumes = new List<VolumeModel>();
            foreach (RedisKey key in keys)
            {
                logger.LogDebug($"Reading the key {key}");
                VolumeModel volumeModel = new VolumeModel();
                foreach (PropertyInfo property in volumeModel.GetType().GetProperties())
                {
                    var value = await db.HashGetAsync(key, property.Name);
                    property.SetValue(volumeModel, Convert.ChangeType(value, property.PropertyType));
                }
                volumes.Add(volumeModel);
            }

            logger.LogDebug($"All volumes ejected successfully");
            return volumes.ConvertAll(v => VolumeAdapter.Volume(v));
        }

        private static string GetVolumeKey(string userId, string volumeId = "*")
        {
            return $"{userId}:{volumeId}";
        }
    }
}
