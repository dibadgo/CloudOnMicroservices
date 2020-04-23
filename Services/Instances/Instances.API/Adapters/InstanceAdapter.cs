using CommonLib;
using Instances.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.API.Adapters
{
    public class InstanceAdapter : IAdapter<Instance, InstanceGrpc>, IAdapter<LaunchInstanceRequest, Instance>
    {    
        public InstanceGrpc Transform(Instance model)
        {
            InstanceGrpc instanceGrpc = new InstanceGrpc()
            {
                Id = model.Id,
                Name = model.Name,
                InstanceType = model.InstanceType,
                SystemVolume = model.SystemVolume.Id,
                State = InstanceStateFrom(model.InstanceState)
            };

            instanceGrpc.DataVolumes.AddRange(model.DataVolumes.ConvertAll(v => v.Id));

            return instanceGrpc;
        }

        public Instance Transform(LaunchInstanceRequest request)
        {
            List<Volume> dataVolumes = request.DataVolumes
               .ToList()
               .ConvertAll(id => new Volume() { Id = id});

            return new Instance()
            {
                Id = IdentityFabric.GenInstanceId(),
                Name = request.Name,                
                SystemVolume = new Volume() { Id = request.SystemVolume },
                DataVolumes = dataVolumes,
                InstanceType = request.InstanceType,
                InstanceState = InstanceState.PENDING
            };
        }

        private InstanceStateGrpc InstanceStateFrom(InstanceState state)
        {
            return (InstanceStateGrpc)(int)state; 
        }
    }
}
