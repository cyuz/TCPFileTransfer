using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;
using ServerLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.MessageHandler
{
    public class GetFileRequestMessageHandler : IServerMessageHandler
    {
        private IMyNetworkAdapter _myNetworkAdapter;

        public GetFileRequestMessageHandler(IMyNetworkAdapter networkAdatper)
        {
            _myNetworkAdapter = networkAdatper;
        }

        public void Handle(IMessage message, IServerContext context, INetworkConnection conn)
        {
            GetFileRequest request = (GetFileRequest)message;

            string filePath = Path.Combine(context.FileFolder, request.FileName);

            bool exist = context.FileUtil.Exists(filePath);

            context.NotifyRequestReceived(conn.IP, conn.Port, request.FileName, DateTime.Now, exist);

            if(!exist)
            {
                _myNetworkAdapter.WriteFileNotFoundResponseIntoNetworkStream(request.FileName, conn);
                return;
            }

            bool canRead = context.FileUtil.CanRead(filePath);

            if(!canRead)
            {
                _myNetworkAdapter.WriteFileCannotReadResponseIntoNetworkStream(request.FileName, conn);
                return;
            }

            _myNetworkAdapter.WriteGotFileResponseIntoNetworkStream(filePath, context.FileUtil, conn);
            _myNetworkAdapter.LoadFileIntoNetworkStream(filePath, context.FileUtil, conn);
        }
    }
}
