using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Network
{
    public interface IMyNetworkStream
    {
        int Read(byte[] buffer, int offset, int size);
        public void Write(byte[] buffer);
        public void Write(byte[] buffer, int offset, int size);
        public void Flush();
        string IP { get; }
        int Port { get; }
        bool DataAvailable { get; }
    }
}
