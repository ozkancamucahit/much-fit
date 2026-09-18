namespace Central.Common.Lib.Interfaces.Streaming;

public interface IStreamSubscriber
{
  Task SubscribeAsync<T>(string topic, Action<T> handler) where T : class;

}
