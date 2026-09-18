using Central.Common.Lib.Interfaces;
using Central.Common.Lib.Interfaces.Streaming;
using StackExchange.Redis;

namespace Central.Common.Lib.Streaming.Redis;

public sealed class RedisStreamPublisher 
  : IStreamPublisher
{
    private readonly ISerializer _serializer;
    private readonly ISubscriber _subscriber;

    public RedisStreamPublisher(
      IConnectionMultiplexer connectionMultiplexer,
      ISerializer serializer)
    {
        _serializer = serializer;
        _subscriber = connectionMultiplexer.GetSubscriber();
    }

  [Obsolete]
  public Task PublishAsync<T>(string topic, T data) where T : class
    {
        var payload = _serializer.Serialize(data);
        return _subscriber.PublishAsync(topic, payload);
    }
}
