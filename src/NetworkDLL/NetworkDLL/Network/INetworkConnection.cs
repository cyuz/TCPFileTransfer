using NetworkDLL.Message;

namespace NetworkDLL.Network
{
    public interface INetworkConnection
    {
        void Flush();
        int Read(byte[] buffer, int offset, int size);
        IMessage ReadMessage();
        void Write(byte[] buffer);
        void Write(byte[] buffer, int offset, int size);
        void WriteMessage(IMessage message);
        string IP { get; }
        int Port { get; }
        void Disconnect();
    }
}