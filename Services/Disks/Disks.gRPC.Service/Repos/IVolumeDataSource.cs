using Disks.gRPC.Service.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Repos
{
    public interface IVolumeDataSource
    {
        Task<VolumeReply> Create(CreateVolumeRequest createVolumeRequest, string userId);

        Task<VolumeReply> Get(string key, string userId);

        Task<IEnumerable<VolumeReply>> List(string userId);
    }
}
