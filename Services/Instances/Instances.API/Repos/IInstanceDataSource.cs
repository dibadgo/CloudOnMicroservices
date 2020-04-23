using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.API.Repos
{
    public interface IInstanceDataSource
    {
        Task<InstanceGrpc> LaunchAsync(LaunchInstanceRequest request, string userId);

        Task<InstanceGrpc> StartAsync(string instanceId, string userId);

        Task<InstanceGrpc> StopAsync(string instanceId, string userId);

        Task<InstanceGrpc> TerminateAsync(string instanceId, string userId);

        IEnumerable<InstanceGrpc> List(string userId);
    }
}
