

namespace Central.Common.Lib.Types.Domain;

public abstract class DomainEventBase : IDomainEvent
{
  public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow;
}
