using ClientLib.Core;
using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;
using System;

namespace ClientLib.MessageHandler
{
    public class GotFileResponseMessageHandler : IClientMessageHandler
    {
        private IMyNetworkAdapter _myNetworkAdapter;

        public GotFileResponseMessageHandler(IMyNetworkAdapter networkAdatper)
        {
            _myNetworkAdapter = networkAdatper;
        }

        public void Handle(IMessage message, IClientContext context, INetworkConnection conn)
        {
            GotFileResponse response = (GotFileResponse)message;

            if(!context.FileUtil.CanWrite(context.FilePath))
            {
                context.NotifyDownloadFailed(DownloadErrorCode.CANNOT_WRITE, response.FileName);
                return;
            }

            bool result = _myNetworkAdapter.SaveFileFromNetworkStream(context.FilePath, response.FileLength, context.FileUtil, conn);
            if (result)
            {
                context.NotifyDownloadComplete(response.FileName);
            }
            else
            {
                context.NotifyDownloadFailed(DownloadErrorCode.DONWLOAD_ERROR, response.FileName);
            }
        }
    }
}
