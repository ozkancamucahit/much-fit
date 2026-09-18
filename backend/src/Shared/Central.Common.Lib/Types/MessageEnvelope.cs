using Central.Common.Lib.Interfaces.Messaging;

namespace Central.Common.Lib.Types;

public record MessageEnvelope<T>(
  T Message,
  string CorrelationId) where T : IMessage;
