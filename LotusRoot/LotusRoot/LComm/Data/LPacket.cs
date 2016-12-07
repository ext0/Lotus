using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.Data
{
    public class LPacket
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LPacket));

        public static readonly int METADATA_LENGTH = 1;
        public static readonly int LENGTH_LENGTH = 4; //yes, really
        public static readonly int DECOMPRESSION_BUFFER_SIZE = 4096;

        private byte[] _data;
        private byte[] _packetBuffer;
        private byte _metadata;

        public LPacket(byte[] structured, LMetadata metadata)
        {
            _data = structured;
            _metadata = (byte)metadata;
            byte[] data;
            using (MemoryStream memory = new MemoryStream(structured.Length))
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, false))
                {
                    gzip.Write(structured, 0, structured.Length);
                }
                data = memory.ToArray();
            }
            byte[] length = BitConverter.GetBytes(data.Length + METADATA_LENGTH);
            _packetBuffer = new byte[length.Length + data.Length + METADATA_LENGTH];
            Buffer.BlockCopy(length, 0, _packetBuffer, 0, length.Length);
            _packetBuffer[length.Length] = _metadata;
            Buffer.BlockCopy(data, 0, _packetBuffer, length.Length + METADATA_LENGTH, data.Length);
        }

        public LPacket(byte[] length, byte metadata, byte[] raw)
        {
            _metadata = metadata;
            if (raw.Length > METADATA_LENGTH)
            {
                using (GZipStream stream = new GZipStream(new MemoryStream(raw), CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[DECOMPRESSION_BUFFER_SIZE];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, DECOMPRESSION_BUFFER_SIZE);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        _data = memory.ToArray();
                    }
                }
            }
            else
            {
                _data = raw;
            }
        }

        public byte[] Packet
        {
            get
            {
                return _packetBuffer;
            }
        }

        public int PacketLength
        {
            get
            {
                return _packetBuffer.Length;
            }
        }

        public byte[] PackagedData
        {
            get
            {
                return _data;
            }
        }

        public int PackagedDataLength
        {
            get
            {
                return _data.Length;
            }
        }

        public LMetadata Metadata
        {
            get
            {
                return (LMetadata)_metadata;
            }
        }

        public override string ToString()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes((Metadata.ToString() + ":" + PackagedDataLength)));
        }
    }
}
