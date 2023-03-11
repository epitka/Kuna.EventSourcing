using Microsoft.Extensions.DependencyInjection;

namespace Kuna.EventSourcing.Core.Configuration;

public interface IServicesConfigurator
{
    IServiceCollection ConfigureServices(IServiceCollection services);
}
