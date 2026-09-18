namespace Central.Common.Lib.Interfaces;

public interface ISerializer
{
  string Serialize<T>(T value);
  T? Deserialize<T>(string value);
  byte[] SerializeBytes<T>(T value);
  T? DeserializeBytes<T>(byte[] value);
}
