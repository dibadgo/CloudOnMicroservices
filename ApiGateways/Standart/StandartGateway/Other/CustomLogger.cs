using EventBus.Base.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StandartGateway.Other
{
    public class CustomLogger : ILogger
    {
        private readonly string _name = "SomeName";
        private readonly IEventBus _eventBus;

        public CustomLogger(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (_config.EventId == 0 || _config.EventId == eventId.Id)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = _config.Color;
                string message = $"{logLevel.ToString()} - {eventId.Id} - {_name} - {formatter(state, exception)}";
                var customEvent = new CustomServiceLogEvent("StandartGateway", message, DateTime.Now);

                _eventBus.Publish(customEvent);

                Console.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {_name} - {formatter(state, exception)}");
                Console.ForegroundColor = color;
            }
        }
    }
}
