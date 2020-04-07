using Disks.gRPC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StandartGateway.Services
{
    public interface IVolumeDataSource
    {
        Task<VolumeReply> GetVolume(string volumeId);

        Task<VolumeReply> CreateVolume(VolumeConfigurationBindModel bindModel);

        Task<List<VolumeReply>> Volumes();
    }
}
