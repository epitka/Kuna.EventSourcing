using Carts.Application;
using Carts.Application.CommandHandlers;
using Carts.Application.Services;
using Carts.Domain.Aggregate;
using Carts.Domain.Commands;
using Carts.Domain.Services;
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
