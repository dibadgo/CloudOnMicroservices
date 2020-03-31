using Disks.gRPC.Service.Data;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Repos
{
    public class VolumesRepository : IVolumeDataSource
    {
        private readonly RedisService redisService;

        public VolumesRepository(RedisService redisService)
        {
            this.redisService = redisService;
        }

        /// <summary>
        /// Create a volume
        /// </summary>
        /// <param name="createVolumeRequest">Parameters for create</param>
        /// <exception cref="VolumeException">If cannot create a volume</exception>
        /// <returns>gRPC reply with volume id</returns>
        public async Task<VolumeReply> Create(CreateVolumeRequest createVolumeRequest)
        {
            string volumeId = EntityIdGenerator.Create();
            VolumeModel volume = VolumeAdapter.Volume(volumeId, createVolumeRequest);
            string json = volume.ToJson();

            using ConnectionMultiplexer redis = redisService.Connect();
            IDatabase db = redis.GetDatabase();

            return await db.StringSetAsync(volumeId, json) 
                ? VolumeAdapter.Volume(volume) 
                : throw new VolumeException("Cannot create a volume");
        }

        public string Get(string key)
        {
            using ConnectionMultiplexer redis = redisService.Connect();
            var db = redis.GetDatabase();
            return db.StringGet(key);
        }
    }
}
