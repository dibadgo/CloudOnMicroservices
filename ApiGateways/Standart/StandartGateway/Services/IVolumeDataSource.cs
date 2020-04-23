using Disks.gRPC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StandartGateway.Services
{
    /// <summary>
    /// Manage volumes
    /// </summary>
    public interface IVolumeDataSource
    {
        /// <summary>
        /// Obtan a volume
        /// </summary>
        /// <param name="volumeId">Volume id</param>
        /// <returns></returns>
        Task<VolumeReply> GetVolume(string volumeId);
        /// <summary>
        /// Volume configuration
        /// </summary>
        /// <param name="bindModel">Volume configuration</param>
        /// <returns></returns>

        Task<VolumeReply> CreateVolume(VolumeConfigurationBindModel bindModel);
        /// <summary>
        /// Get list of volumes for user
        /// </summary>
        /// <returns></returns>

        Task<List<VolumeReply>> Volumes();
    }
}
