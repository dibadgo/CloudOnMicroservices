using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommonLib.Logging
{
    public static class CriticalLoggingServiceExtensions
    {       

        public static ILoggingBuilder CustomLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, CriticalLoggerProvider>();
            return builder;
        }

        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<RabbitMqOptions>();

            return services;
        }
    }
}
