using NetworkDLL.File;
using NetworkDLL.Network;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClientLib.Core
{
    public class FileClientProxy : IFileClientProxy
    {
        private INetworkClient _client;
        private IMyNetworkAdapter _myNetworkAdapter;
        private IFileUtil _fileUtil;

        public event Action OnStart;
        public event Action OnConnectSuccess;
        public event Action<ConnectFailErrorCode> OnConnectFailed;
        public event Action OnStop;
        public event Action<string> OnDownloadComplete;
        public event Action<DownloadErrorCode, string> OnDownloadFailed;

        private void _client_OnDisconnet()
        {
            this.OnStop?.Invoke();
        }

        public FileClientProxy(INetworkClient client, IFileUtil fileUtil, IMyNetworkAdapter myNetworkAdapter)
        {
            _client = client;
            _fileUtil = fileUtil;
            _myNetworkAdapter = myNetworkAdapter;

            this._client.OnDisconnet += _client_OnDisconnet;
        }

        public FileClientProxy() : this(new NetworkClient(), new FileUtil(), new MyNetworkAdapter())
        {           
        }

        public void RequestFile(string ip, int port, string fileFolder, string fileName)
        {
            this.OnStart?.Invoke();

            try
            {
                _client.Connect(ip, port);
            }
            catch (ArgumentNullException)
            {
                this.OnConnectFailed?.Invoke(ConnectFailErrorCode.ARGUMENT_ERROR);
                return;
            }
            catch (SocketException ex)
            {
                if(ex.ErrorCode == 10061)
                {
                    this.OnConnectFailed?.Invoke(ConnectFailErrorCode.CONNECTION_REFUSE);
                }
                else if (ex.ErrorCode == 11001)
                {
                    this.OnConnectFailed?.Invoke(ConnectFailErrorCode.HOST_NOT_FOUND);
                }
                else
                {
                    this.OnConnectFailed?.Invoke(ConnectFailErrorCode.SOCKET_ERROR);
                }
                return;
            }
            catch
            {
                this.OnConnectFailed?.Invoke(ConnectFailErrorCode.UNKNONW_REASON);
                return;
            }

            this.OnConnectSuccess?.Invoke();

            StartListenTask(fileFolder, fileName);
        }

        private void StartListenTask(string fileFolder, string fileName)
        {
            string filePath = Path.Combine(fileFolder, fileName);

            ClientContext context = new ClientContext(this._fileUtil, fileName, filePath);
            context.OnDownloadComplete += Context_OnDownloadComplete;
            context.OnDownloadFailed += Context_OnDownloadFailed;

            Task t = Task.Run(() => ListenForServer(context, this._client, this._myNetworkAdapter));
        }

        private void Context_OnDownloadFailed(DownloadErrorCode arg1, string arg2)
        {
            this.OnDownloadFailed?.Invoke(arg1, arg2);
        }

        private void Context_OnDownloadComplete(string obj)
        {
            this.OnDownloadComplete?.Invoke(obj);
        }

        private static void ListenForServer(ClientContext context, INetworkClient client, IMyNetworkAdapter fileMiddleware)
        {
            FileClientCommunicationStateMachine worker = new FileClientCommunicationStateMachine(context, client.GetConnection(), fileMiddleware);

            try
            {
                while (true)
                {
                    if (!worker.ShouldPump)
                    {
                        break;
                    }
                    worker.Pump();
                }
            }
            catch(Exception)
            {
                ///TODO: LOG
            }

            if(client.Connected)
            {
                client.Disconnect();
            }
        }

        public void Cancel()
        {
            this._client.Disconnect();
        }
    }
}
