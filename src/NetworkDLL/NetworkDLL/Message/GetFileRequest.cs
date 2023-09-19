using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Message
{
    public class GetFileRequest : IMessage
    {
        public MessageTypeEnum MessageType => MessageTypeEnum.GET_FILE_REQUEST;

        public const int FixedPartSize = sizeof(UInt32)/* string len*/;

        public string FileName { get; set; }

        public GetFileRequest()
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

                return ms.ToArray();
            }
        }
    }
}
