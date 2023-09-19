using System;
using System.Net;
using System.Net.Sockets;

namespace NetworkDLL.Network
{
    public class MyTcpClientPxory : ITcpClientPxory
    {
        public event Action OnDisconnet;

        public bool Connected => this._client?.Connected ?? false;

        private TcpClient _client;
        private IMyNetworkStream _stream;

        public MyTcpClientPxory()
        {
        }

        public MyTcpClientPxory(TcpClient client)
        {
            _client = client;
            _client.ReceiveTimeout = Consts.READ_TIMEOUT;
        }

        public void Connect(string ip, int port)
        {
            try
            {
                _client = new TcpClient();               
                _client.ReceiveTimeout = Consts.READ_TIMEOUT;
                _client.Connect(ip, port);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (SocketException)
            {
                throw;
            }
        }

        public void Disconnect()
        {
            if(_client != null)
            {
                _client.Close();
                _client = null;

                this._stream = null;

                this.OnDisconnet?.Invoke();
            }
        }

        public IMyNetworkStream GetStream()
        {
            if (!this.Connected)
            {
                throw new InvalidOperationException("Call connect first");
            }

            if (this._stream == null)
            {
                this._stream = new MyNetworkStream(_client.GetStream());
            }
            return _stream;
        }

        public string IP => ((IPEndPoint)this._client.Client.RemoteEndPoint).Address.ToString();

        public int Port => ((IPEndPoint)this._client.Client.RemoteEndPoint).Port;
    }
}
