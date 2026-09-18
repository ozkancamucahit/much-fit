using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Central.Common.Lib.Interfaces;

namespace Central.Common.Lib;

public sealed class CustomXmlSerializer : IXmlSerializer
{
  private readonly XmlWriterSettings _writerSettings;
  private readonly XmlReaderSettings _readerSettings;
  private readonly Dictionary<Type, XmlSerializer> _serializerCache;

  public CustomXmlSerializer()
  {
    _writerSettings = new XmlWriterSettings
    {
      Indent = true,
      Encoding = Encoding.UTF8,
      CheckCharacters = true,
      OmitXmlDeclaration = false
    };

    _readerSettings = new XmlReaderSettings
    {
      IgnoreWhitespace = true,
      IgnoreComments = true,
      ValidationType = ValidationType.None
    };

    _serializerCache = new Dictionary<Type, XmlSerializer>();
  }

  private XmlSerializer GetSerializer<T>()
  {
    var type = typeof(T);
    if (!_serializerCache.TryGetValue(type, out var serializer))
    {
      serializer = new XmlSerializer(type);
      _serializerCache[type] = serializer;
    }
    return serializer;
  }

  public string Serialize<T>(T obj) where T : class
  {
    if (obj == null)
      throw new ArgumentNullException(nameof(obj));

    try
    {
      using var stringWriter = new StringWriter();
      using var xmlWriter = XmlWriter.Create(stringWriter, _writerSettings);
      var serializer = GetSerializer<T>();

      serializer.Serialize(xmlWriter, obj);
      return stringWriter.ToString();
    }
    catch (Exception ex)
    {
      throw new XmlSerializationException("Failed to serialize object", ex);
    }
  }

  public T Deserialize<T>(string xml) where T : class
  {
    if (string.IsNullOrWhiteSpace(xml))
      throw new ArgumentNullException(nameof(xml));

    try
    {
      using var stringReader = new StringReader(xml);
      using var xmlReader = XmlReader.Create(stringReader, _readerSettings);
      var serializer = GetSerializer<T>();

      return serializer.Deserialize(xmlReader) as T
          ?? throw new XmlSerializationException("Deserialization resulted in null object");
    }
    catch (Exception ex)
    {
      throw new XmlSerializationException("Failed to deserialize XML", ex);
    }
  }

  public async Task<string> SerializeAsync<T>(T obj) where T : class
  {
    if (obj == null)
      throw new ArgumentNullException(nameof(obj));

    try
    {
      await using var memoryStream = new MemoryStream();
      await using var xmlWriter = XmlWriter.Create(memoryStream, _writerSettings);
      var serializer = GetSerializer<T>();

      serializer.Serialize(xmlWriter, obj);
      await xmlWriter.FlushAsync();

      memoryStream.Position = 0;
      using var streamReader = new StreamReader(memoryStream);
      return await streamReader.ReadToEndAsync();
    }
    catch (Exception ex)
    {
      throw new XmlSerializationException("Failed to serialize object asynchronously", ex);
    }
  }

  public async Task<T> DeserializeAsync<T>(string xml) where T : class
  {
    if (string.IsNullOrWhiteSpace(xml))
      throw new ArgumentNullException(nameof(xml));

    try
    {
      await using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
      using var xmlReader = XmlReader.Create(memoryStream, _readerSettings);
      var serializer = GetSerializer<T>();

      var result = serializer.Deserialize(xmlReader) as T;
      return result ?? throw new XmlSerializationException("Deserialization resulted in null object");
    }
    catch (Exception ex)
    {
      throw new XmlSerializationException("Failed to deserialize XML asynchronously", ex);
    }
  }
}

public sealed class XmlSerializationException : Exception
{
  public XmlSerializationException(string message) : base(message) { }
  public XmlSerializationException(string message, Exception innerException)
      : base(message, innerException) { }
}
