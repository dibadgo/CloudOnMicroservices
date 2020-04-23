using CommonLib.Logging;
using CommonLib.Services;
using Disks.gRPC.Service.Repos;
using Disks.gRPC.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;

namespace Disks.gRPC.Service
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
               
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuthService(services);

            services.AddGrpc();
            services.AddSingleton<RedisService>();
            services.AddScoped<IVolumeDataSource, VolumesRepository>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();

            AddLoggerProvider(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<VolumesService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(
                        "Communication with gRPC endpoints must be made through a gRPC client. " +
                        "To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
                        );
                });
            });
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            var identityUrl = Configuration.GetValue<string>("urls:identity"); // Get the Identity

            IdentityModelEventSource.ShowPII = true;

            services.AddAuthorization();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "volumes";
            });
        }

        public IServiceCollection AddLoggerProvider(IServiceCollection services)
        {
            services.AddRabbitMq(Configuration);

            var loggerFactory = new LoggerFactory();

            services.AddLogging(configure => configure.AddConsole());
            services.AddLogging(configure => configure.CustomLogger());

            return services;
        }
    }
}
