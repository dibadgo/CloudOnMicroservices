using Instances.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonLib;

namespace Instances.API.Adapters
{
    public class VolumeAdapter : IAdapter<VolumeGrpc, Volume>, IAdapter<Volume, VolumeGrpc>
    {
        public Volume Transform(VolumeGrpc model)
        {
            return new Volume()
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public VolumeGrpc Transform(Volume model)
        {
            return new VolumeGrpc()
            {
                Id = model.Id,
                Name = model.Name
            };
        }
    }
}
