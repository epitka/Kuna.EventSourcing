using System.Reflection;
using Carts.Application;
using Carts.Application.CommandHandlers;
using Carts.Application.EventHandlers;
using Carts.Application.Services;
using Carts.Domain.Aggregate;
using Carts.Domain.Aggregate.Events;
using Carts.Domain.Commands;
using Carts.Domain.Services;
using Carts.Infrastructure;
using Carts.Infrastructure.Commands;
using EventStore.Client;
using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.EventStore;
using Kuna.EventSourcing.EventStore.Configuration;
using Kuna.EventSourcing.EventStore.Subscriptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Carts;

public class ServicesConfigurator
{
    public IConfiguration Configuration { get; set; } = default!;

    public IHostEnvironment Environment { get; set; } = default!;

    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services
            .AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Carts", Version = "v1" });
                    //// c.OperationFilter<MetadataOperationFilter>();
                })
            .AddControllers();

        Func<Assembly[], Type[]> eventsDiscoveryFunc = assemblies =>
        {
            var interfaceType = typeof(IAggregateEvent);
            var eventTypes = assemblies
                             .SelectMany(i => i.GetTypes())
                             .Where(x => interfaceType.IsAssignableFrom(x))
                             .ToArray();

            return eventTypes;
        };

        services.AddEventStore(
            configuration: this.Configuration,
            eventStoreConnectionStringName: "EventStore",
            assembliesWithAggregateEvents: new[] { typeof(ShoppingCart).Assembly },
            aggregateEventsDiscoverFunc: eventsDiscoveryFunc,
            subscriptionSettings: new[]
            {
                new StreamSubscriptionSettings(
                    "$ce-cart",
                    StreamPosition.Start),
            });

        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<IProductPriceCalculator, RandomProductPriceCalculator>();

        this.AddCommandHandlers(services);

        this.AddEventHandlers(services);

        return services;
    }

    private void AddCommandHandlers(IServiceCollection services)
    {
        // https://github.com/khellang/Scrutor could be used to auto-wire up handlers
        // or you could just wire it up via reflection using naming convention
        services.AddTransient<IHandleCommand<AddProduct>, AddProductHandler>();
        services.AddTransient<IHandleCommand<CancelShoppingCart>, CancelShoppingCartHandler>();
        services.AddTransient<IHandleCommand<ConfirmShoppingCart>, ConfirmShoppingCartHandler>();
        services.AddTransient<IHandleCommand<OpenShoppingCart>, OpenShoppingCartHandler>();
        services.AddTransient<IHandleCommand<RemoveProduct>, RemoveProductHandler>();
    }

    private void AddEventHandlers(IServiceCollection services)
    {
        services.AddTransient<IHandleEvent<ShoppingCartConfirmed>, ShoppingCartConfirmedHandler>();
    }
}
