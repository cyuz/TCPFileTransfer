using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Message
{
    public class MessageHeader
    {
        public const int FixedPartSize = sizeof(UInt32) * 3;

        public int MessageType { get; set; }

        public UInt32 CRC { get; set; }

        public int MessageSize { get; set; }

        public byte[] Serialize()
        {
            using(MemoryStream ms = new MemoryStream())
            using(BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((UInt32)this.MessageType);
                bw.Write(this.CRC);
                bw.Write((UInt32)this.MessageSize);

                return ms.ToArray();
            }
        }

        public int Deserialize(Stream stream)
        {
            using(BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                this.MessageType = (int)br.ReadUInt32();
                this.CRC = br.ReadUInt32();
                this.MessageSize = (int)br.ReadUInt32();


                return (int)br.BaseStream.Position;
            }
        }
    }
}
