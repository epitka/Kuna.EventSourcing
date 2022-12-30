using Microsoft.Extensions.DependencyInjection;

namespace Senf.EventSourcing.Core.Configuration;

public interface IServicesConfigurator
{
    IServiceCollection ConfigureServices(IServiceCollection services);
}
