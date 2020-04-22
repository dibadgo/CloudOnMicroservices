using CommonLib;
using CommonLib.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using StandartGateway.Other;
using StandartGateway.Services;
using System.IO;

namespace StandartGateway
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLoggerProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerFactory = new LoggerFactory();

            services.AddLogging(configure => configure.AddConsole());
            services.AddLogging(configure => configure.CustomLogger());

            return services;
        }

        public static IServiceCollection AddCustomApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UrlsConfig>(configuration.GetSection("urls"));

            //Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Gateway",
                    Version = "v1",
                    Description = "Gateway HTTP API"
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;

                var xmlPath = Path.Combine(basePath, "StandartGateway.xml");
                options.IncludeXmlComments(xmlPath);
            });
            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var identityUrl = configuration.GetValue<string>("urls:identity");
            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "gateway";
            });

            return services;
        }

        public static IServiceCollection AddHttpServices(this IServiceCollection services)
        {
            //register delegating handlers
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //register http services
            services
               .AddHttpClient<GrpcCallerService>()
               .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

            services
                .AddHttpClient<IVolumeDataSource, VolumeService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

            return services;
        }
    }
}
