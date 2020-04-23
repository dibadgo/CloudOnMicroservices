using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.API.Models
{
    public enum InstanceState : int
    {
        STOPPED,
        STOPPING,

        RUNNING,
        STARTING,

        TERMINATED,
        TERMINATING,

        PENDING
    }
}
