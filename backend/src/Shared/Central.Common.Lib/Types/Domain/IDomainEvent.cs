
namespace Central.Common.Lib.Types.Domain;



public interface IDomainEvent
{
  DateTimeOffset DateOccurred { get; }
}
