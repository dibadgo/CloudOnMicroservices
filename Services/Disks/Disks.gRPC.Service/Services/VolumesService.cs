using Disks.gRPC.Service.Repos;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Services
{
    public class VolumesService : Volume.VolumeBase
    {
        private readonly IVolumeDataSource volumeDataSource;

        public VolumesService(IVolumeDataSource volumeDataSource)
        {
            this.volumeDataSource = volumeDataSource;
        }

        public override async Task<VolumeReply> Create(CreateVolumeRequest request, ServerCallContext context)
        {
            return await volumeDataSource.Create(request);
        }

        public override Task<VolumeReply> Get(GetVolume request, ServerCallContext context)
        {
            return base.Get(request, context);
        }

        public override Task<VolumeReply> List(CreateVolumeRequest request, ServerCallContext context)
        {
            return base.List(request, context);
        }
    }
}
