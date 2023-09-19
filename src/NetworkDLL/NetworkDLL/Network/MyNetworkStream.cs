using System;
using System.Net;
using System.Net.Sockets;

namespace NetworkDLL.Network
{
    public class MyNetworkStream : IMyNetworkStream
    {
        private NetworkStream _stream;

        public MyNetworkStream(NetworkStream ns)
        {
            if (ns == null) throw new ArgumentNullException("ns");
            this._stream = ns;
        }

        public bool DataAvailable
        {
            get
            {
                return this._stream.DataAvailable;
            }
        }

        public string IP => ((IPEndPoint)this._stream.Socket.RemoteEndPoint).Address.ToString();

        public int Port => ((IPEndPoint)this._stream.Socket.RemoteEndPoint).Port;

        public int Read(byte[] buffer, int offset, int size)
        {
            return this._stream.Read(buffer, offset, size);
        }

        public void Write(byte[] buffer)
        {
            this._stream.Write(buffer);
        }

        public void Write(byte[] buffer, int offset, int size)
        {
            this._stream.Write(buffer, offset, size);
        }

        public void Flush()
        {
            this._stream.Flush();
        }
    }
}
