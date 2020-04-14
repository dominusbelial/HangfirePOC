using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hangfire
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            GetHangfireDBCreated();

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage("Server=hangfiredb;Database=Hangfire;User=sa;Password=Pass_abcd1234;"));

            //job registration goes here.
            services.AddScoped<Jobs.IMyJob, Jobs.MyJob>();
        }

        //enable remote access and create hangfire database.
        private void GetHangfireDBCreated()
        {
            string dbName = "Hangfire";
            string dbConn = "Server=hangfiredb;Database=master;User=sa;Password=Pass_abcd1234;";

            using (var connexion = new System.Data.SqlClient.SqlConnection(dbConn))
            {
                connexion.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(string.Format(
                    @"EXEC sp_configure 'remote access', 1;
                      RECONFIGURE;
                      IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}') 
                         create database [{0}];
                      ", dbName), connexion))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize([NotNull] DashboardContext context)
            {
                //can add some more logic here...
                return true;
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            var jobServerOptions = new BackgroundJobServerOptions
            {
                WorkerCount = 1
            };
            app.UseHangfireServer(jobServerOptions);

            //recurring jobs (cron) created at startup.
            RecurringJob.AddOrUpdate(() => Console.WriteLine("Minutely Job Running..."), Cron.Minutely);
            RecurringJob.RemoveIfExists(nameof(Jobs.MyJob));
            //runs every 2 minutes
            RecurringJob.AddOrUpdate<Jobs.MyJob>(nameof(Jobs.MyJob), 
                job => job.Run(JobCancellationToken.Null),
                "*/2 * * * *", TimeZoneInfo.Local);

            app.UseHangfireDashboard("/jobs", new DashboardOptions()
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });            
        }
    }
}
