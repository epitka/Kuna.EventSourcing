using System.Data.SqlTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Xunit.Sdk;

namespace Senf.EventSourcing.Testing;

public class DependencyRegistrationVerifier<TProgram>
    where TProgram : class
{
    public class SuccessException : Exception
    {
        public SuccessException(VerificationResult result)
        {
            this.Result = result;
        }

        public VerificationResult Result { get; }
    }

    public class FailureException : Exception
    {
        public FailureException(VerificationResult result)
        {
            this.Result = result;
        }

        public VerificationResult Result { get; }
    }

    public class VerificationResult : Exception
    {
        public HashSet<string> FailureMessages { get; set; } = new();

        public HashSet<Type> FailedTypes { get; set; } = new();

        public Type[] TypesToVerify { get; set; } = Array.Empty<Type>();

        public HashSet<Type> SuccessfullyResolved { get; set; } = new();

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Total types to verify: " + this.TypesToVerify.Length);
            sb.AppendLine("");

            if (this.FailureMessages.Any())
            {
                sb.AppendLine("Failure messages:");
                sb.AppendLine(separator);

                foreach (var item in this.FailureMessages)
                {
                    sb.AppendLine(item);
                }
            }

            sb.AppendLine("");

            if (this.FailedTypes.Any())
            {
                sb.AppendLine("Failed to resolve:");
                sb.AppendLine(separator);

                foreach (var item in this.FailedTypes)
                {
                    sb.AppendLine(item.FullName);
                }
            }

            sb.AppendLine("");
            sb.AppendLine("Resolved successfuly:");
            sb.AppendLine(separator);

            foreach (var item in this.SuccessfullyResolved)
            {
                sb.AppendLine(item.FullName);
            }

            return sb.ToString();
        }
    }

    private readonly IEnumerable<Assembly> assemblies;
    private const string separator = "-----------------------------------------";

    /// <summary>
    ///
    /// </summary>
    /// <param name="assemblies">assemblies that contain handlers and/or controllers</param>
    /// <param name="console">XUnit logging console</param>
    public DependencyRegistrationVerifier(IEnumerable<Assembly> assemblies)
    {
        this.assemblies = assemblies;
    }

    public void Validate(params Type[] typesToVerify)
    {
        var app = new WebApplicationFactory<TProgram>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.ConfigureTestServices(
                        sc =>
                        {
                            var toVerify = this.GetTypesToVerify(typesToVerify)
                                               .ToList();

                            // because controllers are not added to service provider
                            // let's add them so we can try to resolve them
                            var controllers = toVerify.Where(x => x.IsAssignableTo(typeof(Controller)));

                            foreach (var controller in controllers)
                            {
                                sc.AddScoped(controller);
                            }

                            var sp = sc.BuildServiceProvider();

                            var result = InternalValidate(
                                sp,
                                toVerify.ToArray());

                            if (result.FailedTypes.Any())
                            {
                                throw new FailureException(result);
                            }

                            throw new SuccessException(result);
                        });
                });

        app.CreateClient();
    }

    private static VerificationResult InternalValidate(IServiceProvider sp, params Type[] typesToVerify)
    {
        var toReturn = new VerificationResult
        {
            TypesToVerify = typesToVerify,
        };

        using var scope = sp.CreateScope();

        foreach (var item in typesToVerify)
        {
            try
            {
                var result = scope.ServiceProvider.GetServices(item);

                if (!result.Any())
                {
                    throw new Exception($"Could not resolve services for {item.FullName}");
                }

                toReturn.SuccessfullyResolved.Add(item);
            }
            catch (Exception e)
            {
                toReturn.FailureMessages.Add(e.Message);
                toReturn.FailedTypes.Add(item);
            }
        }

        return toReturn;
    }

    private IEnumerable<Type> GetTypesToVerify(IEnumerable<Type> typesToVerify)
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
                    var closedInterface = closedType.GetInterfaces()
                                                    .Single(x => x.GetGenericTypeDefinition() == candidateType);

                    toReturn.Add(closedInterface);
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
