using Central.Common.Lib.Interfaces.Streaming;
using Central.Common.Lib.Streaming.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Central.Common.Lib.Extensions.Redis;

public static class RedisExtentions
{

  public static IServiceCollection AddRedis(
    this IServiceCollection services
    , IConfiguration configuration
    , string redisSectionName = "redis")
  {
    var section = configuration.GetRequiredSection(redisSectionName);

    if (section is null)
    {
      throw new ArgumentNullException("redis coniguration section", "Redis config is not found");
    }

    var options = configuration.GetOptions<RedisOptions>(redisSectionName);

    if (options is null || String.IsNullOrWhiteSpace(options.ConnectionString))
    {
      throw new ArgumentNullException("redis connection string", "Redis connection string is not set");
    }

    services.Configure<RedisOptions>(section);
    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.ConnectionString));

    return services;
  }

  public static WebApplicationBuilder AddAspireRedis(
  this WebApplicationBuilder builder
  , string connectionName = "redis"
  )
  {
    var cnnString = builder.Configuration.GetConnectionString(connectionName);

    if (String.IsNullOrWhiteSpace(cnnString))
    {
      throw new ArgumentNullException("redis connection string", "Redis connection string is not set");
    }

    builder.AddRedisClient(connectionName);
    return builder;
  }

  public static IServiceCollection AddRedisStreaming(this IServiceCollection services)
  {
    return
      services
        .AddSingleton<IStreamPublisher, RedisStreamPublisher>()
        .AddSingleton<IStreamSubscriber, RedisStreamSubscriber>();
  }


}