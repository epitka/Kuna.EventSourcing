
namespace Senf.EventSourcing.Testing;

public class DependencyRegistrationVerifier
{
    private readonly IServiceCollection services;
    private readonly IEnumerable<Assembly> assemblies;
    private readonly ITestOutputHelper console;
    private const string separator = "-----------------------------------------";

    /// <summary>
    ///
    /// </summary>
    /// <param name="services">Build up services collection, usually exposed by ContainerAwareTest<T>/ContainerBasedTest </param>
    /// <param name="assemblies">assemblies that contain handlers and/or controllers</param>
    /// <param name="console">XUnit logging console</param>
    public DependencyRegistrationVerifier(
        IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ITestOutputHelper console)
    {
        this.services = services;
        this.assemblies = assemblies;
        this.console = console;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="resolveDelegate">delegate that is used to resolve service. If using ContainerBasedTest use Resolve method for example  .Run*(this.Resolve)</param>
    /// <param name="typeToVerify"> array of type definitions that you want to verify for, such as IRequestHandler<>, IRequestHandler<,>, INotificationHandler<> if using MediatR,
    /// if you want to verify controllers pass them in as well.  If using EventSourcing.Core then pass IHandleEvent<>, IHandleCommand<>
    /// Example of call using ServiceProvider  .Run(this.GetServices, typeof(IHandleEvent<>), typeof(IHandleCommand<>, typeof(Controller)
    public void Run(Func<Type, object> resolveDelegate, params Type[] typesToVerify)
    {
        var toVerify = this.GetTypesToVerify(typesToVerify);

        // let's first register these types so we can try to resolve each one
        // because they are not registered with ioc, but rather interfaces
        foreach (var type in toVerify)
        {
            this.services.AddScoped(type);
        }

        var pass = true;
        var notResolvedCnt = 0;

        var resolved = new HashSet<string>();
        var failedMessages = new HashSet<string>();
        var failed = new HashSet<string>();

        foreach (var item in toVerify)
        {
            try
            {
                var _ = resolveDelegate(item);
                resolved.Add(item.FullName);
            }
            catch (Exception e)
            {
                failedMessages.Add(e.Message);
                failed.Add(item.FullName);
            }
        }

        var sb = new StringBuilder();

        if (failedMessages.Any())
        {
            foreach (var item in failedMessages)
            {
                sb.AppendLine(item);
            }
        }

        sb.AppendLine(separator);
        sb.AppendLine("");

        sb.AppendLine("Total types to verify: " + toVerify.Count);
        sb.AppendLine("");

        sb.AppendLine("Failed to resolve:");
        sb.AppendLine(separator);
        foreach (var item in failed)
        {
            sb.AppendLine(item);
        }

        sb.AppendLine("");
        sb.AppendLine("Resolved successfuly:");
        sb.AppendLine(separator);

        foreach (var item in resolved)
        {
            sb.AppendLine(item);
        }

        this.console.WriteLine(sb.ToString());

        if (failedMessages.Any())
        {
            throw new Exception("Failed to resolve all types");
        }
    }

    private HashSet<Type> GetTypesToVerify(Type[] typesToVerify)
    {
        var toReturn = new HashSet<Type>();

        // just make sure we get all assemblies where we have command and event handlers
        var exportedTypes = this.assemblies
                                .SelectMany(a => a.GetExportedTypes())
                                .ToArray();

        foreach (var candidateType in typesToVerify)
        {
            if (candidateType.IsGenericTypeDefinition
                && candidateType.IsInterface)
            {
                // get all types that implement open generic interface type such as
                // INotificationHandler<>, IRequest<>, IRequest<,>, IHandleCommand<>, IHandleEvent<>
                var closedTypes = exportedTypes.Where(
                                                   x => x.GetInterfaces()
                                                         .Any(
                                                             y => y.IsGenericType
                                                                  && y.GetGenericTypeDefinition() == candidateType))
                                               .Where(x => x.IsAbstract == false);

                foreach (var closedType in closedTypes)
                {
                    toReturn.Add(closedType);
                }
            }
            else
            {
                var types = exportedTypes.Where(x => candidateType.IsAssignableFrom(x))
                                         .Where(x => x.IsAbstract == false);

                foreach (var type in types)
                {
                    toReturn.Add(type);
                }
            }
        }

        return toReturn;
    }
}
