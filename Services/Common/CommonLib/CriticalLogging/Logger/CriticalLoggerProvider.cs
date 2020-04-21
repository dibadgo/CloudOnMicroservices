using Microsoft.Extensions.Logging;

namespace CommonLib.Logging
{
    public class CriticalLoggerProvider : ILoggerProvider
    {
        private readonly RabbitMqOptions options;

        private RabbitMqLoggerEmiter rabbitMqEmiter;


        public CriticalLoggerProvider(RabbitMqOptions options)
        {
            this.options = options;
        }    

        public ILogger CreateLogger(string categoryName)
        {
            if (rabbitMqEmiter == null)
                rabbitMqEmiter = new RabbitMqLoggerEmiter(options);

            return new CriticalLogger(rabbitMqEmiter, options.EmiterName);
        }

        public void Dispose()
        {
            if (rabbitMqEmiter != null)
                rabbitMqEmiter.Dispose();
        }
    }
}
