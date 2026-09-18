namespace Central.Common.Lib.Interfaces;

public interface IXmlSerializer
{
  string Serialize<T>(T obj) where T : class;
  T Deserialize<T>(string xml) where T : class;
  Task<string> SerializeAsync<T>(T obj) where T : class;
  Task<T> DeserializeAsync<T>(string xml) where T : class;
}
