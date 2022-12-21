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

      /*.AddCorrelationIdMiddleware()
       .AddOptimisticConcurrencyMiddleware(
           sp => sp.GetRequiredService<EventStoreDBExpectedStreamRevisionProvider>().TrySet,
           sp => () => sp.GetRequiredService<EventStoreDBNextStreamRevisionProvider>().Value?.ToString()
       )*/
       .AddControllers();

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

public partial class Program
{
}
