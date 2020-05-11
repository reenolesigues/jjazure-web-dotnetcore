﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jjwebapicore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace jjwebcore
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
            services.AddControllersWithViews();
            services.AddApplicationInsightsTelemetry();

            // client generated by Nswag
            // https://elanderson.net/2019/11/use-http-client-factory-with-nswag-generated-classes-in-asp-net-core-3/
            services.AddHttpClient<IContactsClient, ContactsClient>("contacts", client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVICEAPIROOT_URL"));
            });

            services.AddHttpClient("jjwebapicore", client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVICEAPIROOT_URL"));
            });
            services.AddHttpClient("jjwebwinapicore", client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVICEWINAPIROOT_URL"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
            });
        }
    }
}
