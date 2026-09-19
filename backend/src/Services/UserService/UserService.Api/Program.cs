using Central.Common.Lib.Extensions.Observability;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Platform.Web;
using Platform.Web.Extensions;
using UserService.Api;
using UserService.Api.Auth;
using UserService.Api.Data;
using UserService.Api.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddLogging("UserService");

builder
  .Services
  .AddPlatformExceptionHandling()
  .AddPlatformRateLimiting()
  .AddCorrelationId();


var connectionString = builder.Configuration.GetConnectionString("UserDb");
ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

builder.Services.AddDbContext<UserDbContext>(options =>
{

  if (!builder.Environment.IsProduction())
  {
    var connectionBuilder = new NpgsqlConnectionStringBuilder(connectionString)
    {
      IncludeErrorDetail = true
    };
    connectionString = connectionBuilder.ConnectionString;
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
  }

  options.UseNpgsql(connectionString);
});


builder
  .Services
  .AddIdentityCore<ApplicationUser>(options =>
  {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
  })
  .AddRoles<IdentityRole<Guid>>()
  .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
  .AddEntityFrameworkStores<UserDbContext>();


builder.Services.AddLocalization();

builder.Services.AddControllers()
  .AddDataAnnotationsLocalization(options =>
  {
    options.DataAnnotationLocalizerProvider = (type, factory) =>
      factory.Create(typeof(SharedResource));
  });

builder
  .Services
  .AddScoped<JWTTokenService>();


builder.Services.AddOpenApi();




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app
  .UseCorrelationId()
  .UseExceptionHandler()
  .UseRateLimiter();

var supportedCultures = new[] { "en", "tr" };
var localizationOptions = new RequestLocalizationOptions()
  .SetDefaultCulture("en")
  .AddSupportedCultures(supportedCultures)
  .AddSupportedUICultures(supportedCultures);

localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localizationOptions);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();


app.MapGet("/", () => "User service is running :" + app.Environment.EnvironmentName);

app.Run();
