namespace NetworkDLL.Network
{
    public interface ITcpListenerProxy
    {
        ITcpClientPxory AcceptTcpClient();
        void Start(int port);
        void Stop();
    }
}
