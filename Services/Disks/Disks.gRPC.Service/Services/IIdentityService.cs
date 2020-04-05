using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Services
{
    public interface IIdentityService
    {
        string GetUserIdentity();
    }
}
