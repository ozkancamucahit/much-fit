using Central.Common.Lib.Interfaces.Messaging;
using Central.Common.Lib.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Central.Common.Lib.Extensions.Pulsar;

public static class Extensions
{
  public static WebApplicationBuilder AddPulsar(
  this WebApplicationBuilder builder
  , string connectionName = "pulsar"
  )
  {
    var cnnString = builder.Configuration.GetConnectionString(connectionName);

    if(String.IsNullOrWhiteSpace(cnnString))
    {
      throw new ArgumentNullException("pulsarConnectionString", "Pulsar connection string is null");
    }

    builder.Services.Configure<PulsarOptions>(opt => opt.ConnectionStr = cnnString);

    builder
      .Services
      .AddSingleton<IMessagePublisher, PulsarMessagePublisher>()
      .AddSingleton<IMessageSubscriber, PulsarMessageSubscriber>();

    return builder;
  }
}
