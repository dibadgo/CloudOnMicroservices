using Microsoft.Extensions.Logging;
using System;

namespace CommonLib.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class CriticalLogger : ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly string moduleName;
        /// <summary>
        /// 
        /// </summary>
        private RabbitMqLoggerEmiter emiter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emiter"></param>
        public CriticalLogger(RabbitMqLoggerEmiter emiter, string moduleName)
        {
            this.moduleName = moduleName;
            this.emiter = emiter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LogLevel.Critical || logLevel == LogLevel.Error;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
  
            string message = $"{logLevel} - {eventId.Id} - {moduleName} - {formatter(state, exception)}";
           
            emiter.Send(message);     
        }
    }
}
