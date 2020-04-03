using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Data
{
    public static class VolumeAdapter
    {
        private const string SEPARATOR = "_";

        public static VolumeModel Volume(string id, CreateVolumeRequest createVolumeRequest)
        {
            return new VolumeModel(
                id,
                createVolumeRequest.SizeGb,
                string.Join(SEPARATOR, createVolumeRequest.MountPints),
                createVolumeRequest.Name
            );
        }

        public static VolumeReply Volume(VolumeModel volumeModel)
        {
            VolumeReply reply = new VolumeReply()
            {
                Id = volumeModel.Id,
                Name = volumeModel.Name,
                SizeGb = volumeModel.Size
            };
            reply.MountPoints.AddRange(volumeModel.MountPoints.Split(SEPARATOR));

            return reply;
        }
    }
}
