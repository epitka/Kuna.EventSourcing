using Microsoft.Extensions.DependencyInjection;

namespace Kuna.Utilities.Configuration;

public interface IServicesConfigurator
{
    IServiceCollection ConfigureServices(IServiceCollection services);
}
