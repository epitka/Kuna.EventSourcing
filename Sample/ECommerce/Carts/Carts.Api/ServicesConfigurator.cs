using Carts.Application;
using Carts.Application.CommandHandlers;
using Carts.Application.Services;
using Carts.Domain.Aggregate;
using Carts.Domain.Commands;
using Carts.Domain.Services;
using Carts.Infrastructure;
using EventStore.Client;
using Kuna.EventSourcing.Core.Commands;
using Kuna.EventSourcing.Core.Configuration;
using Kuna.EventSourcing.Core.EventStore.Configuration;
using Kuna.EventSourcing.Core.EventStore.Subscriptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


namespace Carts.Api;

public class ServicesConfigurator : IServicesConfigurator
{
    public IConfiguration Configuration { get; set; } = default!;

    public IHostEnvironment Environment { get; set; } = default!;
    public IServiceCollection ConfigureServices(
        IServiceCollection services)
    {
        services
            .AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Carts", Version = "v1" });
                    //// c.OperationFilter<MetadataOperationFilter>();
                })

            /*.AddCorrelationIdMiddleware()
             .AddOptimisticConcurrencyMiddleware(
                 sp => sp.GetRequiredService<EventStoreDBExpectedStreamRevisionProvider>().TrySet,
                 sp => () => sp.GetRequiredService<EventStoreDBNextStreamRevisionProvider>().Value?.ToString()
             )*/
            .AddControllers();

        services.AddEventStore(
            configuration:this.Configuration,
            eventStoreConnectionStringName:"EventStore",
            assembliesWithAggregateEvents: new [] {typeof(ShoppingCart).Assembly},
            subscriptionSettings: new[]
            {
                new StreamSubscriptionSettings(
                    "$ce-cart",
                    StreamPosition.End),
            });

        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<IProductPriceCalculator, RandomProductPriceCalculator>();

        this.AddCommandHandlers(services);

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
}
