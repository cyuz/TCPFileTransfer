using System.Net;
using System.Net.Sockets;

namespace NetworkDLL.Network
{
    public class MyTcpListenerProxy : ITcpListenerProxy
    {
        private TcpListener _server;

        public MyTcpListenerProxy()
        {

        }


        public ITcpClientPxory AcceptTcpClient()
        {
            TcpClient client = _server?.AcceptTcpClient();
            if(client == null)
            {
                return null;
            }


            return new MyTcpClientPxory(client);
        }

        public void Start(int port)
        {
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();
        }

        public void Stop()
        {
            _server?.Stop();
            _server = null;
        }
    }
}
