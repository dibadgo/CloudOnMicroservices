using CommonLib;
using Instances.API.Database;
using Instances.API.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.API.Repos
{
    /// <summary>
    /// This is a place for instance managment e.g. saveing to DB, searching and changing the state
    /// </summary>
    public class InstancesRepository : IInstanceDataSource
    {
        /// <summary>
        /// EF context to DataBase
        /// </summary>
        private readonly InstanceDbContext instanceDbContext;
        /// <summary>
        /// Adapter from Instance to InstanceGrpc
        /// </summary>
        private readonly IAdapter<Instance, InstanceGrpc> instanceAdapter;
        /// <summary>
        /// Adapter from LaunchInstanceRequest to Instance
        /// </summary>
        private readonly IAdapter<LaunchInstanceRequest, Instance> launchInstanceAdapter;
        /// <summary>
        /// Custom logger
        /// </summary>
        private readonly ILogger<InstancesRepository> logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instanceDbContext">EF context to DataBase</param>
        /// <param name="instanceAdapter">Adapter from Instance to InstanceGrpc</param>
        /// <param name="launchInstanceAdapter">Adapter from LaunchInstanceRequest to Instance</param>
        /// <param name="logger">Custom logger</param>
        public InstancesRepository(
            InstanceDbContext instanceDbContext, 
            IAdapter<Instance, InstanceGrpc> instanceAdapter,
            IAdapter<LaunchInstanceRequest, Instance> launchInstanceAdapter,
            ILogger<InstancesRepository> logger)
        {
            this.instanceDbContext = instanceDbContext;
            this.instanceAdapter = instanceAdapter;
            this.launchInstanceAdapter = launchInstanceAdapter;
            this.logger = logger;
        }

        /// <summary>
        /// Launch an instance from configuration
        /// </summary>
        /// <param name="request">Instance configuration</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<InstanceGrpc> LaunchAsync(LaunchInstanceRequest request, string userId)
        {
            Instance instance = launchInstanceAdapter.Transform(request);
            instance.UserId = userId;

            logger.LogDebug($"Created the instance {instance}");

            await instanceDbContext.AddAsync(instance);
            await instanceDbContext.SaveChangesAsync();

            logger.LogDebug($"The instance with Id {instance.Id} successfully save to the DB");

            return instanceAdapter.Transform(instance);
        }
        /// <summary>
        /// Starts the specified instance
        /// </summary>
        /// <param name="instanceId">Instance Id</param>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public async Task<InstanceGrpc> StartAsync(string instanceId, string userId)
        {
            Instance instance = await SetInstanceState(instanceId, userId, InstanceState.STOPPED);

            logger.LogInformation($"The instance {instanceId} successfully stopped");
            return instanceAdapter.Transform(instance);
        }
        /// <summary>
        /// Stop the specified instance
        /// </summary>
        /// <param name="instanceId">Instance Id</param>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public async Task<InstanceGrpc> StopAsync(string instanceId, string userId)
        {
            Instance instance = await SetInstanceState(instanceId, userId, InstanceState.RUNNING);

            logger.LogInformation($"The instance {instanceId} successfully started");
            return instanceAdapter.Transform(instance);
        }
        /// <summary>
        /// Terminate the specified instance
        /// </summary>
        /// <param name="instanceId">Instance Id</param>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public async Task<InstanceGrpc> TerminateAsync(string instanceId, string userId)
        {
            Instance instance = await SetInstanceState(instanceId, userId, InstanceState.TERMINATED);

            logger.LogInformation($"The instance {instanceId} successfully terminated");
            return instanceAdapter.Transform(instance);
        }
        /// <summary>
        /// List all instance by user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public IEnumerable<InstanceGrpc> List(string userId)
        {
            IQueryable<Instance> instances = from i in instanceDbContext.Instances
                                             where i.UserId == userId
                                             select i;

            logger.LogInformation($"Found {instances.Count()} instances for user {userId}");
            return instances
                .ToList()
                .ConvertAll(i => instanceAdapter.Transform(i));
        }
        /// <summary>
        /// Set the instance state
        /// </summary>
        /// <param name="instanceId">Instance Id</param>
        /// <param name="userId">User Id</param>
        /// <param name="instanceState">Instance state</param>
        /// <returns></returns>
        private async Task<Instance> SetInstanceState(string instanceId, string userId, InstanceState instanceState)
        {
            logger.LogDebug($"Searching the instance by Id {instanceId}, userId {userId} and NOT in terminated state");

            Instance instance = instanceDbContext.Instances
                                .Where(i => i.Id == instanceId && i.UserId == userId && i.InstanceState != InstanceState.TERMINATED)
                                .FirstOrDefault();

            if (instance == null)
                throw new InstanceException($"Cannot find instance {instanceId}");

            logger.LogDebug($"Try to set the state {instanceState} to the instance {instanceId}");
            instance.InstanceState = instanceState;

            await instanceDbContext.SaveChangesAsync();

            logger.LogDebug($"instance state {instanceState} successfully changed for {instanceId}");
            return instance;
        }
    }
}
