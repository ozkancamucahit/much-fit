namespace Central.Common.Lib.Mvc;

public sealed class ServiceId 
  : IServiceId
{
  private static readonly string UniqueId = $"{Guid.CreateVersion7():N}";

  public string Id => UniqueId;
}
