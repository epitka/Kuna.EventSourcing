using Carts.Application;
using Carts.Domain.Aggregate;
using Carts.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Senf.EventSourcing.Core.Commands;
using Senf.EventSourcing.Core.Configuration;
using Senf.EventSourcing.Core.EventStore.Configuration;

namespace Carts.Api;

public class ServicesConfigurator : IServicesConfigurator
{
    public  IConfiguration Configuration { get; set; }

    public IHostEnvironment Environment { get; set; }
    public IServiceCollection ConfigureServices(
        IServiceCollection services)
    {
        services
            .AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Carts", Version = "v1" });
                    // c.OperationFilter<MetadataOperationFilter>();
                })

            /*.AddCorrelationIdMiddleware()
             .AddOptimisticConcurrencyMiddleware(
                 sp => sp.GetRequiredService<EventStoreDBExpectedStreamRevisionProvider>().TrySet,
                 sp => () => sp.GetRequiredService<EventStoreDBNextStreamRevisionProvider>().Value?.ToString()
             )*/
            .AddControllers();

        services.AddEventStore(this.Configuration, "EventStore", new [] {typeof(ShoppingCart).Assembly});

        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

        return services;
    }
}
