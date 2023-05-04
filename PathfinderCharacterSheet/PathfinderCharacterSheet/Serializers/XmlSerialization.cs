using System.IO;
using System.Xml.Serialization;

namespace PathfinderCharacterSheet
{
    public class XmlSerialization<T>: ISerialization<T>
    {
        public string Format => "xml";

        public T Deserialize(Stream stream)
        {
            return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
        }

        public void Serialize(T data, Stream stream)
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, data);
        }
    }
}
