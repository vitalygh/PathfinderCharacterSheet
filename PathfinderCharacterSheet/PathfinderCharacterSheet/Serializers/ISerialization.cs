using System.IO;

namespace PathfinderCharacterSheet
{
    public interface ISerialization<T>
    {
        string Format { get; }
        T Deserialize(Stream stream);
        void Serialize(T data, Stream stream);
    }
}
