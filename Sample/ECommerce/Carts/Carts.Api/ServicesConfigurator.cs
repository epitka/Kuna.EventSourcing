using Carts.Application;
using Carts.Application.CommandHandlers;
using Carts.Application.EventHandlers;
using Carts.Application.Services;
using Carts.Domain.Aggregate;
using Carts.Domain.Aggregate.Events;
using Carts.Domain.Commands;
using Carts.Domain.Services;
using Carts.Infrastructure;
using Kuna.EventSourcing.Core.Commands;
using Kuna.EventSourcing.Core.Configuration;
using Kuna.EventSourcing.Core.Events;
using Kuna.EventSourcing.Core.EventStore.Configuration;
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

        services.AddEventStore(this.Configuration, "EventStore", new [] {typeof(ShoppingCart).Assembly});

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
