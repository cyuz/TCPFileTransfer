using System;

namespace NetworkDLL.Network
{
    public interface INetworkClient
    {
        event Action OnDisconnet;

        public bool Connected { get; }
        void Connect(string ip, int port);
        void Disconnect();
        INetworkConnection GetConnection();
    }
}