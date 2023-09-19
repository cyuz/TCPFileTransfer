using System;

namespace NetworkDLL.Network
{
    public interface ITcpClientPxory
    {
        public event Action OnDisconnet;

        public bool Connected { get; }

        public void Connect(string ip, int port);

        public IMyNetworkStream GetStream();

        public void Disconnect();

        string IP { get; }

        int Port { get; }
    }
}
