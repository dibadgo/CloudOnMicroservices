using Instances.API;
using StandartGateway.BindModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StandartGateway.Services
{
    /// <summary>
    /// Manage of instances
    /// </summary>
    public interface IInstanceDataSource
    {
        /// <summary>
        /// Launch a new instance by the provided confiduration
        /// </summary>
        /// <param name="configuration">Configurations</param>
        /// <returns></returns>
        Task<InstanceGrpc> LaunchAsync(LaunchInstanceConfiguration configuration);

        /// <summary>
        /// Start the instance
        /// </summary>
        /// <param name="instanceId">The instance id</param>
        /// <returns></returns>
        Task<InstanceGrpc> StartAsync(string instanceId);
        /// <summary>
        /// Stop the instance
        /// </summary>
        /// <param name="instanceId">The instance id</param>
        /// <returns></returns>
        Task<InstanceGrpc> StopAsync(string instanceId);

        /// <summary>
        /// Terminate the instance
        /// </summary>
        /// <param name="instanceId">Instance id</param>
        /// <returns></returns>
        Task<InstanceGrpc> TerminateAsync(string instanceId);
    }
}
