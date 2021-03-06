﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using jjwebapicore.Models;
using System.Diagnostics.CodeAnalysis;
using Prometheus;

namespace jjwebapicore
{
    [ExcludeFromCodeCoverage]
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
            services.AddApplicationInsightsTelemetry();

            services.AddControllers();

            services.AddSwaggerDocument();

            // load connection string from ENV or from appsettings.json
            string connStr = Environment.GetEnvironmentVariable("ConnectionStrings_ContactsContext");
            if (string.IsNullOrEmpty(connStr))
                connStr = Configuration.GetConnectionString("ContactsContext");

            services.AddDbContext<ContactsContext>(options =>
                    options.UseSqlServer(connStr, options => options.EnableRetryOnFailure()));

            services.AddHealthChecks()
                .AddDbContextCheck<ContactsContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ContactsContext> ();
                context.Database.EnsureCreated();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Prometheus server metrics
            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi(c =>
            {
                c.Path = "/api/swagger/v1/swagger.json";
            });
            app.UseSwaggerUi3(c =>
            {
                c.Path = "/api/swagger";
                c.DocumentPath = "/api/swagger/v1/swagger.json";
            });

        }
    }
}
