using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.API.Models
{
    public class InstanceException : Exception
    {
        public InstanceException(string message) : base(message) { }

        public InstanceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
