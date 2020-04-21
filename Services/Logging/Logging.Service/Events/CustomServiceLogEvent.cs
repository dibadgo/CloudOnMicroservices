using EventBus.Base.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logging.Service
{
    public class CustomServiceLogEvent : IntegrationEvent
    {
        public string ModuleName { get; set; }

        public string Message { get; set; }

        public DateTime Time { get; set; }

        public LogType LogType { get; set; }

        public CustomServiceLogEvent(string moduleName, string message, DateTime time, LogType logType)
        {
            ModuleName = moduleName;
            Message = message;
            Time = time;
            LogType = logType;
        }

        public CustomServiceLogEvent()
        {
        }
    }

    public enum LogType
    {
        INFO, ERROR
    }
}
