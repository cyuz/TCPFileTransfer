using NetworkDLL.Message;
using System;

namespace NetworkDLL.Network
{
    public class NetworkConnection : INetworkConnection
    {
        private INetworkClient _client;
        private IMyNetworkStream _stream;

        public NetworkConnection(INetworkClient client, IMyNetworkStream stream)
        {
            this._client = client;
            this._stream = stream;
        }

        public IMessage ReadMessage()
        {
            byte[] data = _stream.ReadPacket();

            if (data == null)
            {
                return null;
            }

            IMessage message = Serializer.Desirialize(data);

            return message;
        }

        public void WriteMessage(IMessage message)
        {
            byte[] data = Serializer.Serialize(message);

            byte[] lengthData = BitConverter.GetBytes((UInt32)data.Length);

            _stream.Write(lengthData);
            _stream.Write(data);
            _stream.Flush();
        }

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

        public void Disconnect()
        {
            this._client.Disconnect();
        }

        public string IP { get => this._stream.IP; }
        public int Port { get => this._stream.Port; }
    }
}
