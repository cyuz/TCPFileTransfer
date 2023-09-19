using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Message
{
    public interface IMessage
    {
        public MessageTypeEnum MessageType { get; }

        public int Deserialize(Stream stream, int numBytesRead);

        public byte[] Serialize();
    }
}
