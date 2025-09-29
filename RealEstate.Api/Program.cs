using RealEstate.Api.Utilities;
using RealEstate.Application;
using RealEstate.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

builder.GetJwtAuthentication();
builder.ConfigServicesDependencyInjection();
builder.Services.AddApplicationDependencies(builder.Configuration);
builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
   {
       options.AddDocumentTransformer((document, context, cancellationToken) =>
       {
           document.Info = new()
           {
               Title = "Real Estate API",
               Version = "v1",
               Description = "API for processing Information for Real State."
           };
           
           if (document.Servers != null)
           {
               foreach (var server in document.Servers)
               {
                   if (!string.IsNullOrEmpty(server.Url) && server.Url.EndsWith("/"))
                   {
                       server.Url = server.Url.TrimEnd('/');
                   }
               }
           }
           
           return Task.CompletedTask;
       });
   });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();
app.Run();