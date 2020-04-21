using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logging.Service
{
    public class RabbitMqOptions
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }
        public string Routekey { get; set; }
    }
}
