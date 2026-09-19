using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using UserService.Api.Data;

namespace UserService.Test;

public sealed class UserServiceTestFactory
  : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly string _databaseName = $"UserDb_test{Guid.CreateVersion7():N}";
  private readonly RSA _testRSA = RSA.Create(keySizeInBits: 2048);
  private string connectionString =>
    $"Host=localhost;Port=5444;Database={_databaseName};Username=root;Password=root;IncludeErrorDetail=True";


  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseSetting("Jwt:PrivateKeyPem", _testRSA.ExportPkcs8PrivateKeyPem());
    builder.UseSetting("Jwt:PublicKeyPem", _testRSA.ExportSubjectPublicKeyInfoPem());
    builder.ConfigureAppConfiguration((_, config) =>
    {
      config.AddInMemoryCollection(new Dictionary<string, string?>
      {
        ["ConnectionStrings:UserDb"] = connectionString,
      });
    });
    base.ConfigureWebHost(builder);
  }

  public async Task InitializeAsync()
  {
    using var scope = Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await db.Database.MigrateAsync().ConfigureAwait(false);
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    using (var scope = Services.CreateScope())
    {
      var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
      await db.Database.EnsureDeletedAsync().ConfigureAwait(false);
    }
    _testRSA.Dispose();
    await base.DisposeAsync().ConfigureAwait(false);
  }
}
