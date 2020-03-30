using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disks.gRPC.Service
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly int _redisPort;
        private readonly Logger<RedisService> logger;

        public RedisService(IConfiguration config, Logger<RedisService> logger)
        {
            _redisHost = config["Redis:Host"];
            _redisPort = Convert.ToInt32(config["Redis:Port"]);
            this.logger = logger;
        }

        public ConnectionMultiplexer Connect()
        {
            try
            {
                var configString = $"{_redisHost}:{_redisPort},connectRetry=5";
                return ConnectionMultiplexer.Connect(configString);
            }
            catch (RedisConnectionException err)
            {
                logger.LogError(err.ToString());
                throw err;
            }
        }
    }
}
