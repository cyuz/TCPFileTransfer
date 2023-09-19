using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Network
{
    public class NetworkServer : INetworkServer
    {
        private ITcpListenerProxy _server;

        public NetworkServer(ITcpListenerProxy server)
        {
            this._server = server;
        }

        public NetworkServer()
        {
            this._server = new MyTcpListenerProxy();
        }

        public INetworkClient AcceptNetworkClient()
        {
            var client = this._server.AcceptTcpClient();
            return new NetworkClient(client);
        }

        public void Start(int port)
        {
            this._server.Start(port);
        }

        public void Stop()
        {
            this._server.Stop();
        }
    }
}
