using StackExchange.Redis;

namespace Disks.gRPC.Service
{
    public interface IRedisConnectionService
    {
        IConnectionMultiplexer Connect();
    }
}