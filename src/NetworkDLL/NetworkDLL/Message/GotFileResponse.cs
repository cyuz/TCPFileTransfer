using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Message
{
    public class GotFileResponse : IMessage
    {
        public MessageTypeEnum MessageType => MessageTypeEnum.GOT_FILE_RESPONSE;

        public const int FixedPartSize = sizeof(UInt32)/* string len*/ +sizeof(long);

        public string FileName { get; set; }
        public long FileLength { get; set; }

        public GotFileResponse()
        {

        }

        public int Deserialize(Stream stream, int numBytesRead)
        {
            using (BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                br.BaseStream.Seek(numBytesRead, SeekOrigin.Begin);

                int fileNameLength = (int)br.ReadUInt32();
                byte[] fileNameRawData = br.ReadBytes(fileNameLength);

                this.FileName = UTF8Encoding.UTF8.GetString(fileNameRawData);

                this.FileLength = (long)br.ReadUInt64();

                return (int)br.BaseStream.Position;
            }
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                byte[] fileNameRawData = UTF8Encoding.UTF8.GetBytes(this.FileName);

                bw.Write((UInt32)fileNameRawData.Length);
                bw.Write(fileNameRawData);
                bw.Write((UInt64) FileLength);

                return ms.ToArray();
            }
        }
    }
}
