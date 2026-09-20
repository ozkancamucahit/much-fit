using ApiGateway.Api;
using Central.Common.Lib.Extensions.Observability;
using Platform.Auth.Extensions;
using Platform.Web;
using Platform.Web.Extensions;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.AddServiceDefaults();
builder
  .AddLogging("ApiGateway");

builder
  .Services
  .AddPlatformExceptionHandling()
  .AddPlatformRateLimiting()
  .AddCorrelationId();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder
  .Services
  .AddReverseProxy()
  .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder
  .Services
  .AddPlatformJwtAuthentication(builder.Configuration, builder.Environment);

builder
  .Services
  .AddAuthorizationBuilder()
  .AddPolicy("authenticated", policy => policy.RequireAuthenticatedUser());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app
  .UseSecurityHeaders()
  .UseHttpsRedirection()
  .UseCorrelationId()
  .UseSerilogRequestLogging(options =>
  {
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
      diagnosticContext.Set("CorrelationId", httpContext.GetCorrelationId());
    options.GetLevel = (httpContext, _, _) =>
      httpContext.Request.Path.StartsWithSegments("/health") || httpContext.Request.Path.StartsWithSegments("/alive")
        ? LogEventLevel.Debug
        : LogEventLevel.Information;
  })
  .UseExceptionHandler()
  .UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Gateway is running :" + app.Environment.EnvironmentName);

app.MapReverseProxy();
app.Run();
