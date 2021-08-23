using Bulkie.BuildingBlocks.EventBus;
using Bulkie.BuildingBlocks.EventBus.Abstractions;
using BulkieFileProcessor.API.Infrastructure;
using BulkieFileProcessor.API.IntegrationEvents.EventHandlers;
using BulkieFileProcessor.API.Model;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Minio;
using Minio.AspNetCore;
using Minio.AspNetCore.HealthChecks;
using System;
using System.Reflection;
using System.Text.Json;

namespace BulkieFileProcessor.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services
                .AddControllers()
                .AddDapr(options =>
                {
                    options.UseJsonSerializationOptions(new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });
                });

            services.AddCustomDbContext(Configuration);

            services.AddMinio(options =>
            {
                options.Endpoint = $"{Configuration["Minio:Host"]}:{Configuration["Minio:Port"]}";
                options.AccessKey = Configuration["Minio:AccessKey"];
                options.SecretKey = Configuration["Minio:SecretKey"];
            });

            services.AddScoped<IBlobRepository, BlobRepository>();
            services.AddScoped<IFileReferenceRepository, FileReferenceRepository>();

            services.AddCustomHealthCheck(Configuration);
             
            services.AddScoped<IEventBus, DaprEventBus>();

            services.AddTransient<BulkieFileStatusChangedToSubmittedIntegrationEventHandler>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BulkieFileProcessor.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BulkieFileProcesor.API v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseCloudEvents();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
                endpoints.MapSubscribeHandler();

                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });

            });
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder
                .AddMinio(sp => sp.GetRequiredService<MinioClient>())
                .AddNpgSql(
                    configuration["ConnectionString"],
                    name: "self",
                    tags: new string[] { "filereferences-db" });

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FileReferenceContext>(options =>
            {
                options.UseNpgsql(configuration["ConnectionString"],
                    npgsqlOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    })
                .UseLowerCaseNamingConvention();
            },
                ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            );

            return services;
        }
    }
}
