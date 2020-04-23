using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StandartGateway.BindModels
{
    /// <summary>
    /// The configuration for a new instance
    /// </summary>
    public class LaunchInstanceConfiguration
    {
        /// <summary>
        /// Instance name
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Instance type
        /// </summary>
        [Required]
        public string InstanceType { get; set; }
        /// <summary>
        /// System volume
        /// </summary>
        [Required]
        public string SystemVolumeId { get; set; }
        /// <summary>
        /// List of data volumes
        /// </summary>        
        public List<string> DataVolumeIds { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public LaunchInstanceConfiguration()
        {
            DataVolumeIds = new List<string>();
        }
    }
}
