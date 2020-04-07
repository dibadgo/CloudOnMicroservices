using Disks.gRPC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StandartGateway.Services
{
    public interface IVolumeSevice
    {
        Task<VolumeReply> CreateVolume();
    }
}
