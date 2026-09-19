using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using UserService.Api.Data;
using UserService.Api.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
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
  .AddEntityFrameworkStores<UserDbContext>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();


app.MapGet("/", () => "User service is running :" + app.Environment.EnvironmentName);

app.Run();
