
using Central.Common.Lib.Interfaces;
using Central.Common.Lib.Interfaces.Streaming;
using StackExchange.Redis;

namespace Central.Common.Lib.Streaming.Redis;

public sealed class RedisStreamSubscriber
  : IStreamSubscriber
{
  private readonly ISerializer _serializer;
  private readonly ISubscriber _subscriber;

  public RedisStreamSubscriber(
    IConnectionMultiplexer connectionMultiplexer,
    ISerializer serializer)
  {
    _serializer = serializer;
    _subscriber = connectionMultiplexer.GetSubscriber();
  }

  [Obsolete]
  public Task SubscribeAsync<T>(
    string topic,
    Action<T> handler) where T : class
  {


    return _subscriber.SubscribeAsync(topic, (_, data) =>
      {
        if (data.IsNullOrEmpty)
        {
          return;
        }

        var payload = _serializer.Deserialize<T>(data!);
        if (payload is null)
        {
          return;
        }

        handler(payload);
      });
  }

}
