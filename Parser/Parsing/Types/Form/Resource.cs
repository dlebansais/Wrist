using System;
using System.IO;

namespace Parser
{
    public class Resource : IResource, IConnectable
    {
        public Resource(string name, string xamlName, string filePath)
        {
            Name = name;
            XamlName = xamlName;
            FilePath = filePath;
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public string FilePath { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public bool Connect(IDomain domain)
        {
            bool IsConnected = false;

            if (Width == 0 || Height == 0)
            {
                IsConnected = true;
                ConnectSize();
            }

            return IsConnected;
        }

        private void ConnectSize()
        {
            IParsingSource Source = new ParsingSource(FilePath, null, 0);

            try
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        ConnectSize(Source, br);
                    }
                }
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new ParsingException(185, Source, "Invalid PNG file.");
            }
        }

        private void ConnectSize(IParsingSource source, BinaryReader br)
        {
            byte[] Signature = br.ReadBytes(8);
            if (Signature[1] != 0x50 || Signature[2] != 0x4E || Signature[3] != 0x47)
                throw new ParsingException(185, source, "Invalid PNG file.");

            byte[] Chunk = br.ReadBytes(8);
            int ChunkLength = BitConverter.ToInt32(Chunk, 0);
            if (ChunkLength < 13)
                throw new ParsingException(185, source, "Invalid PNG file.");

            byte[] ChunkData = br.ReadBytes(13);

            Width =  (((((ChunkData[0] << 8) + ChunkData[1]) << 8) + ChunkData[2]) << 8) + ChunkData[3];
            Height = (((((ChunkData[4] << 8) + ChunkData[5]) << 8) + ChunkData[6]) << 8) + ChunkData[7];

            if (Width <= 0 || Height <= 0)
                throw new ParsingException(185, source, "Invalid PNG file.");
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
