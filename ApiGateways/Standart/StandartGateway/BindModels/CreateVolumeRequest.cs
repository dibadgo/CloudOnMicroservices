using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StandartGateway
{
    /// <summary>
    /// Configuration for creating the volume
    /// </summary>
    public class VolumeConfigurationBindModel
    {
        /// <summary>
        /// The volume name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The size of the volume
        /// </summary>
        public int SizeGb { get; set; }
        /// <summary>
        /// The Os type on the volume
        /// </summary>
        public string OsType { get; set; }
        /// <summary>
        /// All mount points on the disk
        /// </summary>
        public List<string> MountPints { get; set; }
    }
}
