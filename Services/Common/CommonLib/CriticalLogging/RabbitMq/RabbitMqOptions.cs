using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class RabbitMqOptions
    {
        public RabbitMqOptions(IConfiguration config)
        {
            HostName = config["RabbitMq:HostName"];
            Port = Convert.ToInt32(config["RabbitMq:Port"]);
            Exchange = config["RabbitMq:Exchange"];
            Queue = config["RabbitMq:Queue"];
            Routekey = config["RabbitMq:Routekey"];
            EmiterName = config["RabbitMq:EmiterName"];
            IsActive = Convert.ToBoolean(config["RabbitMq:IsActive"]);
        }

        public string HostName { get; set; }
        public int Port { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }
        public string Routekey { get; set; }
        public string EmiterName { get; set; }
        public bool IsActive { get; set; }
    }
}
