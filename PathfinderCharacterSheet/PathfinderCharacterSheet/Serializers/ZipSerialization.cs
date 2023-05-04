using System.IO;
using System.IO.Compression;

namespace PathfinderCharacterSheet
{
    public class ZipSerialization<T>: ISerialization<T>
    {
        private string ContentFilename => "content." + serialization?.Format;
        private readonly ISerialization<T> serialization = null;
        public ZipSerialization(ISerialization<T> serialization)
        {
            this.serialization = serialization;
        }

        public string Format => serialization?.Format + ".zip";

        public T Deserialize(Stream stream)
        {
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var entry = archive.GetEntry(ContentFilename);
                return serialization.Deserialize(entry?.Open());
            }
        }

        public void Serialize(T data, Stream stream)
        {
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                var contentFile = archive.CreateEntry(ContentFilename, CompressionLevel.Optimal);
                using (var entryStream = contentFile.Open())
                {
                    using (var serializationStream = new MemoryStream())
                    {
                        serialization?.Serialize(data, serializationStream);
                        serializationStream.Position = 0;
                        serializationStream.WriteTo(entryStream);
                    }
                }
            }
       }
    }
}
