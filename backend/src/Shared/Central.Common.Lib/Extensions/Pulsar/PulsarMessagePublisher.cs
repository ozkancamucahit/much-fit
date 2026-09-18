using System.Buffers;
using System.Collections.Concurrent;
using System.Reflection;
using Central.Common.Lib.Extensions.Observability;
using Central.Common.Lib.Interfaces;
using Central.Common.Lib.Interfaces.Messaging;
using Central.Common.Lib.Options;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Central.Common.Lib.Extensions.Pulsar;

public sealed class PulsarMessagePublisher
  : IMessagePublisher
{
  //TODO: Extract Pulsar into app settings and dedicated options type
  private readonly ConcurrentDictionary<string, IProducer<ReadOnlySequence<byte>>> _producers = new();
  private readonly ISerializer _serializer;
  private readonly IHttpContextAccessor _contextAccessor;

  private readonly ILogger<PulsarMessagePublisher> _logger;
  private readonly PulsarOptions _pulsarOptions;
  private readonly IPulsarClient _client;
  private readonly string _producerName;

  public PulsarMessagePublisher(
    ISerializer serializer,
    ILogger<PulsarMessagePublisher> logger,
    IOptions<PulsarOptions> pulsarOptions,
    IHttpContextAccessor contextAccessor)
  {
    _serializer = serializer;
    _logger = logger;
    _pulsarOptions = pulsarOptions.Value;
    _client = PulsarClient
      .Builder()
      .ServiceUrl(new Uri(_pulsarOptions.ConnectionStr))
      .Build();

    _logger.LogCritical("connected pulsar with cnn : {cnn}", _pulsarOptions.ConnectionStr);

    _producerName = Assembly.GetEntryAssembly()?.FullName?.Split(",")[0].ToLowerInvariant() ?? string.Empty;
    _contextAccessor = contextAccessor;
  }

  public async Task PublishAsync<T>(string topic, T message) where T : class, Interfaces.Messaging.IMessage
  {
    var producer = _producers.GetOrAdd(topic, _client.NewProducer()
        .ProducerName(_producerName)
        .Topic($"persistent://public/default/{topic}")
        .Create());

    var correlationId = _contextAccessor.HttpContext?.GetCorrelationId()
      ?? Guid.CreateVersion7().ToString("N");

    var payload = _serializer.SerializeBytes(message);
    var metadata = new MessageMetadata
    {
      ["custom_id"] = Guid.CreateVersion7().ToString("N"),
      ["producer"] = _producerName,
      ["correlationId"] = correlationId,
    };
    var messageId = await producer.Send(metadata, payload);
    _logger.LogInformation($"Sent a message with ID: '{messageId}'");
  }
}
