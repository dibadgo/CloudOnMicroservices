﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.API.Models
{
    public class Volume
    {
        public string Id { get; set; }
        
        public string Name { get; set; }

        public string MountPoints { get; set; }
    }
}
