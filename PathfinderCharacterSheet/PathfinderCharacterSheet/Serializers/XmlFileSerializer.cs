using System.IO;
using System.Xml.Serialization;

namespace PathfinderCharacterSheet
{
    public class XmlFileSerializer<T> : FileSerializer<T> where T : class
    {
        protected override string Extension => "xml";

        public XmlFileSerializer(string path) : base(path)
        {
        }

        protected override T Deserialize(StreamReader stream)
        {
            return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
        }

        protected override void Serialize(T data, MemoryStream stream)
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, data);
            stream.Position = 0;
        }
    }
}
