
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carts.Api.Controllers;
using Carts.Application;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Senf.EventSourcing.Core.Commands;
using Senf.EventSourcing.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Carts.Api.Tests;

public class ConfigurationTest : ContainerDrivenTest
{
    private readonly ITestOutputHelper console;

    public ConfigurationTest(ITestOutputHelper console)
    {
        this.console = console;

        var services = new ServiceCollection();
        var configurator = new ServicesConfigurator();

        this.InitializeOriginalServices(() => configurator.ConfigureServices(services));
    }

    [Fact]
    public void VerifyDependencyRegistrations()
    {
        var assemblies = new []
        {
            typeof(ShoppingCartsController).Assembly,  // api
            typeof(IShoppingCartRepository).Assembly,  // application
        };


        var verifier = new DependencyRegistrationVerifier(this.Services, assemblies, this.console);

        verifier.Run(
            this.GetServices,
            typeof(Controller),
            typeof(IHandleCommand<>));
    }
}

