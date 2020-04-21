using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StandartGateway.Other
{
    public class CustomServiceLogEvent : IntegrationEvent
    {
        public string ModuleName { get; set; }

        public string Message { get; set; }

        public DateTime Time { get; set; }

        public CustomServiceLogEvent(string moduleName, string message, DateTime time)
        {
            ModuleName = moduleName;
            Message = message;
            Time = time;
        }

        public CustomServiceLogEvent()
        {
        }
    }
}
