
namespace Central.Common.Lib.Types.Domain;

public abstract class EntityBase : HasDomainEventsBase
{
  public int Id { get; set; }
}

public abstract class EntityBase<TId> : HasDomainEventsBase
  where TId : struct, IEquatable<TId>
{
  public TId Id { get; set; } = default!;
}
