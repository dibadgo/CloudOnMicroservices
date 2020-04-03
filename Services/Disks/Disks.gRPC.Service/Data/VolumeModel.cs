using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Data
{
      public class VolumeModel
    {
        /// <summary>
        /// Disk Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Disk size in GB
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Array of mount points
        /// </summary>
        public string MountPoints { get; set; }
        /// <summary>
        /// Name of the disk
        /// </summary>
        public string Name { get; set; }

        public VolumeModel(string id, int size, string mountPoints, string name)
        {
            Id = id;
            Size = size;
            MountPoints = mountPoints;
            Name = name;
        }
        public VolumeModel()
        {

        }
    }
}
