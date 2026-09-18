namespace Central.Common.Lib.Interfaces.Streaming;

public interface IStreamPublisher
{
  Task PublishAsync<T>(string topic, T data) where T : class;
}
