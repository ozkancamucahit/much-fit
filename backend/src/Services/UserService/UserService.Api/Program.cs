using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Api.Data;
using UserService.Api.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
  .Services
  .AddDbContext<UserDbContext>(options =>
  {
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"));
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
