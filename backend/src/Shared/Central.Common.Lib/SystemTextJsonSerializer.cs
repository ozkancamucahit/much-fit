using System.Text.Json;
using Central.Common.Lib.Interfaces;

namespace Central.Common.Lib;

public sealed class SystemTextJsonSerializer : ISerializer
{
  private static readonly JsonSerializerOptions Options = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
  };

  public string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

  public T? Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value, Options);

  public byte[] SerializeBytes<T>(T value) => JsonSerializer.SerializeToUtf8Bytes(value, Options);

  public T? DeserializeBytes<T>(byte[] value) => JsonSerializer.Deserialize<T>(value, Options);
}
