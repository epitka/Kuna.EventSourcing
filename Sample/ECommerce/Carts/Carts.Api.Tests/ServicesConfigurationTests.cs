using Carts.Api.Controllers;
using Carts.Application;
using Kuna.EventSourcing.Core.Commands;
using Kuna.EventSourcing.Testing;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace Carts.Api.Tests;

public class ServicesConfigurationTest
{
    private readonly ITestOutputHelper console;

    public ServicesConfigurationTest(ITestOutputHelper console)
    {
        this.console = console;
    }

    [Fact]
    public void VerifyDependencyRegistrations()
    {
        var assemblies = new[]
        {
            typeof(ShoppingCartsController).Assembly, // api
            typeof(IShoppingCartRepository).Assembly, // application
        };

        var verifier = new DependencyRegistrationVerifier<Program>(assemblies);

        try
        {
            verifier.Validate(
                typeof(Controller),
                typeof(IHandleCommand<>));
        }
        catch (DependencyRegistrationVerifier<Program>.FailureException fe)
        {
            this.console.WriteLine(fe.Result.ToString());

            Assert.Fail("Could not resolve all dependencies");
        }
        catch (DependencyRegistrationVerifier<Program>.SuccessException e)
        {
            // short cut app bootstrap process
            // ignore
            this.console.WriteLine(e.Result.ToString());
        }
    }
}
