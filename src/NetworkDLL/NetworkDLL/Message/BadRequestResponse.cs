using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Message
{
    public class BadRequestResponse : IMessage
    {
        public MessageTypeEnum MessageType => MessageTypeEnum.BAD_REQUEST_RESPONSE;

        public const int FixedPartSize = sizeof(UInt32) + sizeof(UInt32)/*string len*/;

        public RequestErrorCode ErrorCode { get; set; }

        public string FileName { get; set; }
      
        public BadRequestResponse()
        {

        }

        public int Deserialize(Stream stream, int numBytesRead)
        {
            using (BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                br.BaseStream.Seek(numBytesRead, SeekOrigin.Begin);

                this.ErrorCode = (RequestErrorCode)br.ReadUInt32();

                int messageLength = (int)br.ReadUInt32();
                byte[] meesageRawData = br.ReadBytes(messageLength);

                this.FileName = UTF8Encoding.UTF8.GetString(meesageRawData);

                return (int)br.BaseStream.Position;
            }
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((UInt32)this.ErrorCode);

                byte[] messageRawData = UTF8Encoding.UTF8.GetBytes(this.FileName);

                bw.Write((UInt32)messageRawData.Length);
                bw.Write(messageRawData);

                return ms.ToArray();
            }
        }
    }
}
