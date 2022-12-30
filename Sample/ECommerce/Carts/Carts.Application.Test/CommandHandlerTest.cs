﻿using Carts.Application;
using Carts.Application.CommandHandlers;
using Carts.Domain.Commands;
using Carts.TestingInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using Senf.EventSourcing.Core.Commands;
using Senf.EventSourcing.Testing;

namespace Carts.Tests;
public class ApplicationServicesConfigurator
{
    public IServiceCollection Configure(IServiceCollection services)
    {
        services.AddScoped<IShoppingCartRepository, FakeShoppingCartRepository>();
        services.AddScoped<IHandleCommand<OpenShoppingCart>, OpenShoppingCartHandler>();
        services.AddScoped<OpenShoppingCartHandler>();

        return services;
    }
}

public class CommandHandlerTest : ContainerDrivenTest {
    public CommandHandlerTest()
    {
        var services = new ServiceCollection();

        var configurator = new ApplicationServicesConfigurator();

        this.InitializeOriginalServices(()=>configurator.Configure(services));
    }
}


