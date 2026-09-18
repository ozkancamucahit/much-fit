using Central.Common.Lib.Types;

namespace Central.Common.Lib.Interfaces.Messaging;

public interface IMessageSubscriber
{
  Task SubscribeAsync<T>(
    string topic,
    Action<MessageEnvelope<T>> handler) where T : class, IMessage;
}
