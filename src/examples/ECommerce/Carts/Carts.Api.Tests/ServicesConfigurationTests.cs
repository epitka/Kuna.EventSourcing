using Carts.Api.Controllers;
using Carts.Application;
using Carts.Tests.Exceptions;
using Kuna.EventSourcing.Kurrent;
using Kuna.Extensions.DependencyInjection.Validation;
using Kuna.Utilities.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Carts.Tests;

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

        var verifier = new RegistrationValidator<Program>(assemblies);

        try
        {
            verifier.Validate(
                typeof(Controller),
                typeof(IHandleCommand<>),
                typeof(IHandleEvent<>));
        }
        catch (FailureException fe)
        {
            this.console.WriteLine(fe.Result.ToString());

            Assert.Fail("Could not resolve all dependencies");
        }
        catch (SuccessException e)
        {
            // short cut app bootstrap process
            // ignore
            this.console.WriteLine(e.Result.ToString());
        }
    }
}
