using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool Set(string key, string value)
        {
            using ConnectionMultiplexer redis = redisService.Connect();
            var db = redis.GetDatabase();
            return db.StringSet(key, value);
        }

        public string Get(string key)
        {
            using ConnectionMultiplexer redis = redisService.Connect();
            var db = redis.GetDatabase();
            return db.StringGet(key);
        }
    }
}
