using NetworkDLL.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL
{
    public static class Serializer
    {
        public static byte[] Serialize(IMessage message)
        {
            byte[] content = message.Serialize();

            MessageHeader header = new MessageHeader
            {
                MessageType = (int)message.MessageType,
                MessageSize = content.Length,
                CRC = 0 ///TODO: CRC
            };
            ///TODO: CRC
            byte[] headerContent = header.Serialize();

            using (MemoryStream ms = new MemoryStream())
            using(BinaryWriter bw = new BinaryWriter(ms))
            {
                ms.Write(headerContent);
                ms.Write(content);

                return ms.ToArray();
            }
        }

        public static IMessage Desirialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                MessageHeader header = new MessageHeader();
                int numBytesRead = header.Deserialize(ms);

                MessageTypeEnum messageType = (MessageTypeEnum)header.MessageType;

                if (!Enum.IsDefined(typeof(MessageTypeEnum), messageType))
                {
                    throw new ArgumentException("Unknown Message Type");
                }

                if(header.MessageSize + MessageHeader.FixedPartSize > data.Length)
                {
                    throw new ArgumentException("Message Size over data length");
                }

                ///TODO:CRC

                IMessage message = null;

                switch (messageType)
                {
                    case MessageTypeEnum.GET_FILE_REQUEST:
                        message = new GetFileRequest();
                        break;
                    case MessageTypeEnum.BAD_REQUEST_RESPONSE:
                        message = new BadRequestResponse();
                        break;
                    case MessageTypeEnum.GOT_FILE_RESPONSE:
                        message = new GotFileResponse();
                        break;
                    default:
                        break;
                }

                message?.Deserialize(ms, numBytesRead);


                return message;
            }
        }
    }
}
