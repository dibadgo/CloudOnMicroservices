using System;

namespace Disks.gRPC.Service.Data
{
    public class VolumeException: Exception
    {
        public VolumeException(string message) : base(message) { }
       
        public VolumeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
