using System;
using Carts.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
             .Enrich.FromLogContext()
             .WriteTo.Console()
             .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddJsonFile("appsettings.json");

    builder.Host.UseSerilog(
        (context, services, configuration) => configuration
                                              .ReadFrom.Configuration(context.Configuration)
                                              .ReadFrom.Services(services)
                                              .Enrich.FromLogContext()
                                              .WriteTo.Console());

    var servicesConfigurator = new ServicesConfigurator
    {
        Configuration = builder.Configuration,
        Environment = builder.Environment,
    };

    servicesConfigurator.ConfigureServices(builder.Services);

    var app = builder.Build();

    app
        .UseSerilogRequestLogging()
        .UseRouting()
        .UseAuthorization()
        .UseEndpoints(endpoints => { endpoints.MapControllers(); })
        .UseSwagger()
        .UseSwaggerUI(
            c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Carts V1");
                c.RoutePrefix = string.Empty;
            });

    app.Run();


}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

namespace Carts.Api
{
    public partial class Program
    {
    }
}
