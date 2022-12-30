using Carts.Api;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var servicesConfigurator = new ServicesConfigurator
{
    Configuration = builder.Configuration,
    Environment = builder.Environment,
};

servicesConfigurator.ConfigureServices(
    builder.Services);

var app = builder.Build();

app
    /*.UseExceptionHandlingMiddleware(exception => exception switch
   {
       AggregateNotFoundException _  => HttpStatusCode.NotFound,
       WrongExpectedVersionException => HttpStatusCode.PreconditionFailed,
       _                             => HttpStatusCode.InternalServerError
   })
   .UseCorrelationIdMiddleware()
   .UseOptimisticConcurrencyMiddleware()*/
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
   });

app.Run();

namespace Carts.Api
{
    public partial class Program
    {
    }
}
