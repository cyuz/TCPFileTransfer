using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Network
{
    public class NetworkClient : INetworkClient
    {
        public event Action OnDisconnet;

        private ITcpClientPxory _client;
        private INetworkConnection _conn;

        public bool Connected => this._client?.Connected ?? false;

        public NetworkClient(ITcpClientPxory client)
        {
            this._client = client;
            this._client.OnDisconnet += _client_OnDisconnet;
        }

        public NetworkClient()
        {
            this._client = new MyTcpClientPxory();
            this._client.OnDisconnet += _client_OnDisconnet;
        }

        private void _client_OnDisconnet()
        {
            this.OnDisconnet?.Invoke();
        }

        public void Connect(string ip, int port)
        {
            this._client.Connect(ip, port);
        }

        public INetworkConnection GetConnection()
        {
            if(!this.Connected)
            {
                throw new InvalidOperationException("Call connect first");
            }
            if(this._conn == null)
            {
                this._conn = new NetworkConnection(this, this._client.GetStream());
            }
            return this._conn;
        }

        public void Disconnect()
        {
            this._client.Disconnect();
            this._conn = null;
        }

        string IP { get => this._client.IP; }

        int Port { get => this._client.Port; }
    }
}
