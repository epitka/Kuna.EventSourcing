using Microsoft.Extensions.DependencyInjection;
using Senf.EventSourcing.Core.Configuration;

namespace Senf.EventSourcing.Testing;

public abstract class ContainerDrivenTest<TServicesConfigurator>
    where TServicesConfigurator : IServicesConfigurator, new()
{
    private static readonly Lazy<IServiceCollection?> LazyOriginalServices = new(InitializeOriginalServices);

    private static IServiceCollection? originalServices;

    private static IServiceCollection? OriginalServices => LazyOriginalServices.Value;

    protected ContainerDrivenTest()
    {
        // memoize original services
        this.Services = new ServiceCollection();

        foreach (var descriptor in OriginalServices!)
        {
            this.Services.Add(descriptor);
        }

        this.ConfigureTestServices(this.Services);
    }

    protected IServiceCollection Services { get; }

    private IServiceScope? Scope { get; set; }

    /// <summary>
    /// Configure additional services that are defined outside of the dependency graph of the project being tested.
    /// For example if there are interfaces that are implemented in upper layers or if interfaces are used,
    /// but wiring up happens in upper layers you would also specify it here.
    /// </summary>
    protected abstract void ConfigureTestServices(IServiceCollection services);

    /// <summary>
    /// Replaces service of Type T with your own version (fake,stub,mock,double...)
    /// Lifetime of the service being replace will be used.
    /// </summary>
    /// <typeparam name="T">Type to replace in container.</typeparam>
    /// <param name="obj">Instance to replace with</param>
    /// <returns>ServiceCollection after replacing the service</returns>
    protected IServiceCollection ReplaceService<T>(T obj)
        where T : class
    {
        this.EnsureContainerNotBuilt();

        var serviceDescriptor = this.GetServiceDescriptor(typeof(T));

        this.Services.Remove(serviceDescriptor);

        switch (serviceDescriptor.Lifetime)
        {
            case ServiceLifetime.Scoped:
                this.Services.AddScoped(p => obj);
                break;

            case ServiceLifetime.Singleton:
                this.Services.AddSingleton(p => obj);
                break;

            case ServiceLifetime.Transient:
                this.Services.AddTransient(p => obj);
                break;
            default:
                throw new ArgumentException($"Unknown ServiceLifeTime:{serviceDescriptor.Lifetime}");
        }

        return this.Services;
    }

    protected IServiceCollection ReplaceService<TAbstraction, TImplementation>(TImplementation obj)
        where TAbstraction : class
        where TImplementation : class, TAbstraction
    {
        this.EnsureContainerNotBuilt();

        var serviceDescriptor = this.GetServiceDescriptor(typeof(TAbstraction));

        this.Services.Remove(serviceDescriptor);

        switch (serviceDescriptor.Lifetime)
        {
            case ServiceLifetime.Scoped:
                this.Services.AddScoped<TAbstraction, TImplementation>(p => obj);
                break;
            case ServiceLifetime.Singleton:
                this.Services.AddSingleton<TAbstraction, TImplementation>(p => obj);
                break;
            case ServiceLifetime.Transient:
                this.Services.AddTransient<TAbstraction, TImplementation>(p => obj);
                break;
            default:
                throw new ArgumentException($"Unknown ServiceLifeTime:{serviceDescriptor.Lifetime}");
        }

        return this.Services;
    }

    protected T GetRequiredService<T>()
        where T : notnull
    {
        this.EnsureContainerBootstrapped();

        var service = this.Scope!.ServiceProvider.GetRequiredService<T>();

        return service!;
    }

    protected IEnumerable<object?> GetServices(Type t)
    {
        this.EnsureContainerBootstrapped();

        return this.Scope!.ServiceProvider.GetServices(t);
    }

    private static IServiceCollection? InitializeOriginalServices()
    {
        if (originalServices != null)
        {
            return originalServices;
        }

        originalServices = new ServiceCollection();

        var configurator = new TServicesConfigurator();

        if (configurator is null)
        {
            throw new Exception("Services Configurator is null");
        }

        configurator.ConfigureServices(originalServices);

        return originalServices;
    }

    private void EnsureContainerBootstrapped()
    {
        this.Scope ??= this.Services.BuildServiceProvider().CreateScope();
    }

    private void EnsureContainerNotBuilt()
    {
        if (this.Scope != null)
        {
            throw new InvalidOperationException(
                "Container already built, you cannot replace services after calling any of the methods that resolve services from container such as GetRequiredService or GetServices. Check your test.");
        }
    }

    private ServiceDescriptor GetServiceDescriptor(Type serviceType)
    {
        var serviceDescriptor = this.Services.FirstOrDefault(descriptor => descriptor.ServiceType == serviceType);

        if (serviceDescriptor == null)
        {
            throw new Exception($"Could not locate service {serviceType.Name} to replace. Check you registrations.");
        }

        return serviceDescriptor;
    }
}
