using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Carts", Version = "v1" });
       // c.OperationFilter<MetadataOperationFilter>();
    })
    //.AddKafkaProducer()
    //.AddCoreServices()
    //.AddCartsModule(builder.Configuration)
    //.AddCorrelationIdMiddleware()
    /*.AddOptimisticConcurrencyMiddleware(
        sp => sp.GetRequiredService<MartenExpectedStreamVersionProvider>().TrySet,
        sp => () => sp.GetRequiredService<MartenNextStreamVersionProvider>().Value?.ToString()
    )*/
    .AddControllers()
    // .AddNewtonsoftJson(opt => opt.SerializerSettings.WithDefaults())
    ;

var app = builder.Build();

/*app.UseExceptionHandlingMiddleware(exception => exception switch
    {
        AggregateNotFoundException _ => HttpStatusCode.NotFound,
        ConcurrencyException => HttpStatusCode.PreconditionFailed,
        _ => HttpStatusCode.InternalServerError
    })
    .UseCorrelationIdMiddleware()
    .UseOptimisticConcurrencyMiddleware()
    .UseRouting()
    .UseAuthorization()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    })
    .UseSwagger()
    .UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Carts V1");
        c.RoutePrefix = string.Empty;
    });*/

app.Run();

public partial class Program
{
}
