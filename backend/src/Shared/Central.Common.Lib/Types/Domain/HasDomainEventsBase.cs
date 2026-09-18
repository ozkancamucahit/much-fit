using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Central.Common.Lib.Types.Domain;


public abstract class HasDomainEventsBase : IHasDomainEvents
{
  private readonly List<IDomainEvent> _domainEvents = new();
  [NotMapped]
  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  public int Version { get; protected set; }

  protected void RegisterDomainEvent(DomainEventBase domainEvent)
  {
    if (!_domainEvents.Any())
    {
      Version++;
    }
    _domainEvents.Add(domainEvent);
  }
  public void ClearDomainEvents() => _domainEvents.Clear();

  public abstract string toJsonString(); // => JsonSerializer.Serialize(this, AppJsonContext.Default.HasDomainEventsBase);

  public abstract byte[] toUtf8Bytes(); // => JsonSerializer.SerializeToUtf8Bytes(this, AppJsonContext.Default.HasDomainEventsBase);
}

[JsonSourceGenerationOptions(
  PropertyNameCaseInsensitive = true, 
  WriteIndented = true,
  DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(HasDomainEventsBase))]
internal partial class AppJsonContext : JsonSerializerContext {}
