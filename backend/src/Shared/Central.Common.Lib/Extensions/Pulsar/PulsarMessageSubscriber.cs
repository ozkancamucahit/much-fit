using System.Reflection;
using Central.Common.Lib.Interfaces;
using Central.Common.Lib.Interfaces.Messaging;
using Central.Common.Lib.Options;
using Central.Common.Lib.Types;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Central.Common.Lib.Extensions.Pulsar;

public sealed class PulsarMessageSubscriber
  : IMessageSubscriber
{
  private readonly ISerializer _serializer;
  private readonly ILogger<PulsarMessageSubscriber> _logger;
  private readonly PulsarOptions _options;
  private readonly IPulsarClient _client;
  private readonly string _consumerName;

  public PulsarMessageSubscriber(
    ISerializer serializer,
    ILogger<PulsarMessageSubscriber> logger,
    IOptions<PulsarOptions> options)
  {
    _serializer = serializer;
    _logger = logger;
    _options = options.Value;
    _client = PulsarClient
      .Builder()
      .ServiceUrl(new Uri(_options.ConnectionStr))
      .Build();
    _consumerName = Assembly.GetEntryAssembly()?.FullName?.Split(",")[0].ToLowerInvariant() ?? string.Empty;

  }

  public async Task SubscribeAsync<T>(
    string topic,
    Action<MessageEnvelope<T>> handler) where T : class, Interfaces.Messaging.IMessage
  {
    var subscription = $"{_consumerName}_{topic}";
    var consumer = _client.NewConsumer()
        .SubscriptionName(subscription)
        .Topic($"persistent://public/default/{topic}")
        .Create();

    await foreach (var message in consumer.Messages())
    {
      var producer = message.Properties["producer"];
      var customId = message.Properties["custom_id"];
      var correlationId = message.Properties["correlationId"];
      _logger.LogInformation($"Received a message with ID: '{message.MessageId}' from: '{producer}' " +
                              $"with custom ID: '{customId}'.");
      var payload = _serializer.DeserializeBytes<T>(message.Data.FirstSpan.ToArray());
      if (payload is not null)
      {
        var json = _serializer.Serialize(payload);
        _logger.LogInformation(json);
        handler(new MessageEnvelope<T>(payload, correlationId));
      }

      await consumer.Acknowledge(message);
    }
  }
}
