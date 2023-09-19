using NetworkDLL.File;
using NetworkDLL.Network;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerLib.Core
{
    public class FileServerProxy : IFileServerProxy
    {
        public event Action OnStart;
        public event Action<StartFailedCode> OnStartFailed;
        public event Action OnStop;
        public event Action<string, int, string, DateTime, bool> OnReceiveRequest;

        private IFileUtil _fileUtil;
        private INetworkServer _server;
        private IMyNetworkAdapter _myNetworkAdapter;

        public FileServerProxy(INetworkServer server, IFileUtil fileUtil, IMyNetworkAdapter myNetworkAdapter)
        {
            _fileUtil = fileUtil;
            _server = server;
            _myNetworkAdapter = myNetworkAdapter;
        }

        public FileServerProxy()
        {
            _fileUtil = new FileUtil();
            _server = new NetworkServer();
            _myNetworkAdapter = new MyNetworkAdapter();
        }


        public void Start(int port, string fileFolder)
        {
            try
            {
                _server.Start(port);
            }
            catch(SocketException ex)
            {
                if(ex.ErrorCode == 10048)
                {
                    this.OnStartFailed?.Invoke(StartFailedCode.PORT_OCCUPIED);
                }
                else
                {
                    this.OnStartFailed?.Invoke(StartFailedCode.SOCKET_ERROR);
                }
                return;
            }
            catch(Exception)
            {
                this.OnStartFailed?.Invoke(StartFailedCode.UNKNONW_REASON);
                return;
            }

            this.OnStart?.Invoke();

            StartAcceptClientTask(fileFolder);
        }

        private void StartAcceptClientTask(string fileFolder)
        {
            ServerContext context = new ServerContext(this._fileUtil, fileFolder);
            context.OnReceiveRequest += Context_OnReceiveRequest;

            Task t = Task.Run(() => ListenForClientConnect(context, this._server, this._myNetworkAdapter));
        }

        private void Context_OnReceiveRequest(string arg1, int arg2, string arg3, DateTime arg4, bool arg5)
        {
            this.OnReceiveRequest?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        private static void ListenForClientConnect(ServerContext context, INetworkServer server, IMyNetworkAdapter myNetworkAdapter)
        {
            FileServerAcceptWorker worker = new FileServerAcceptWorker(context, server, myNetworkAdapter);

            while (true)
            {
                if(!worker.Pump())
                {
                    break;
                }
            }
        }

        public void Stop()
        {
            _server.Stop();

            this.OnStop?.Invoke();
        }
    }
}
