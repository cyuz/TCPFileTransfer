using NetworkDLL;
using NetworkDLL.File;
using NetworkDLL.Network;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerLib.Core
{
    public class FileServerAcceptWorker
    {
        private IServerContext _context;
        private INetworkServer _server;
        private IMyNetworkAdapter _myNetworkAdapter;


        public FileServerAcceptWorker(IServerContext context, INetworkServer server, IMyNetworkAdapter myNetworkAdapter)
        {
            _context = context;
            _server = server;
            _myNetworkAdapter = myNetworkAdapter;
        }

        public bool Pump()
        {
            try
            {
                INetworkClient client = this._server.AcceptNetworkClient();

                if(client == null)
                {
                    return false;
                }

                CreateMessageHandlerTask(client);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CreateMessageHandlerTask(INetworkClient client)
        {
            Task t = Task.Run(() => ListenForClient(this._context, client, this._myNetworkAdapter));
        }

        private static void ListenForClient(IServerContext context, INetworkClient client, IMyNetworkAdapter myNetworkAdapter)
        {
            FileServerCommunicationStateMachine worker = new FileServerCommunicationStateMachine(context, client.GetConnection(), myNetworkAdapter);

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
                ///TODO:LOG
            }


            Thread.Sleep(Consts.READ_TIMEOUT);

            if(client.Connected)
            {
                client.Disconnect();
            }
        }
    }
}
