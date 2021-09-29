using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Ac.Api.Gateway
{
    class Program
    {
        static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddJsonFile("ocelot.json")
                        .AddEnvironmentVariables();
                })
                .ConfigureServices(s => {
                    s.AddOcelot();
                    s.AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                        
                        options.Authority = "https://localhost:44394";
                        options.Audience = "ApiRepo";
                    });
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                     //add your logging
                 })
                .UseIISIntegration()
                .Configure(app =>
                {
                    app.UseOcelot().Wait();
                })
                .Build()
                .Run();
        }
    }
}
