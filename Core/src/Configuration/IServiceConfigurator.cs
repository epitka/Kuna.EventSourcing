using Microsoft.Extensions.DependencyInjection;

namespace Senf.EventSourcing.Core.Configuration;

public interface IServicesConfigurator
{
    void ConfigureServices(IServiceCollection services);
}
