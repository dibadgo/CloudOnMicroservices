using Microsoft.Extensions.Logging;
using System;

namespace CommonLib.Logging
{
    /// <summary>
    /// The logger for deliver the criticals and errors massages to logger service
    /// </summary>
    public class CriticalLogger : ILogger
    {
        /// <summary>
        /// The name of module-pusher
        /// </summary>
        private readonly string moduleName;
        /// <summary>
        /// The rabbitMq emmiter
        /// </summary>
        private readonly RabbitMqLoggerEmiter rabbitMqEmiter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emiter">RabbitMqEmiter</param>
        /// <param name="moduleName">The module name</param>
        public CriticalLogger(RabbitMqLoggerEmiter emiter, string moduleName)
        {
            this.moduleName = moduleName;
            this.rabbitMqEmiter = emiter;
        }

        /// <summary>
        /// Begins a logical operation scope 
        /// (Not implemented)
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
        /// <summary>
        /// Checks if the given logLevel is enabled.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LogLevel.Critical || logLevel == LogLevel.Error;
        }
        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The entry to be written</typeparam>
        /// <param name="logLevel">Log-level</param>
        /// <param name="eventId">EventId</param>
        /// <param name="state">State</param>
        /// <param name="exception">Exception</param>
        /// <param name="formatter">Log formetter</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))            
                return;            
  
            string message = $"{logLevel} - {eventId.Id} - {moduleName} - {formatter(state, exception)}";
           
            rabbitMqEmiter.Send(message);     
        }
    }
}
