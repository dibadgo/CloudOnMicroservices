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
        private readonly IAdapter<Volume, VolumeGrpc> VolumeAdapter;

        private readonly IAdapter<VolumeGrpc, Volume> VolumeAdapterGrpc;

        public InstanceAdapter(IAdapter<Volume, VolumeGrpc> volumeAdapter, IAdapter<VolumeGrpc, Volume> volumeAdapterGrpc)
        {           
            VolumeAdapter = volumeAdapter;
            VolumeAdapterGrpc = volumeAdapterGrpc;
        }

        public InstanceGrpc Transform(Instance model)
        {
            InstanceGrpc instanceGrpc = new InstanceGrpc()
            {
                Id = model.Id,
                Name = model.Name,
                InstanceType = model.InstanceType,
                SystemVolume = VolumeAdapter.Transform(model.SystemVolume),
                State = InstanceStateFrom(model.InstanceState)
            };

            var dataVolumes = model.DataVolumes
                .ConvertAll(v => VolumeAdapter.Transform(v));
            instanceGrpc.DataVolumes.Add(dataVolumes);

            return instanceGrpc;
        }

        public Instance Transform(LaunchInstanceRequest request)
        {
            List<Volume> dataVolumes = request.DataVolumes
               .ToList()
               .ConvertAll(v => VolumeAdapterGrpc.Transform(v));

            return new Instance()
            {
                Id = IdentityFabric.GenInstanceId(),
                Name = request.Name,                
                SystemVolume = VolumeAdapterGrpc.Transform(request.SystemVolume),
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
