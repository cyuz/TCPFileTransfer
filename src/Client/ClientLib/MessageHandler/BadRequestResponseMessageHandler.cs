using ClientLib.Core;
using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;
using System;

namespace ClientLib.MessageHandler
{
    public class BadRequestResponseMessageHandler : IClientMessageHandler
    {
        private IMyNetworkAdapter _myNetworkAdapter;

        public BadRequestResponseMessageHandler(IMyNetworkAdapter networkAdatper)
        {
            _myNetworkAdapter = networkAdatper;
        }

        public void Handle(IMessage message, IClientContext context, INetworkConnection conn)
        {
            BadRequestResponse response = (BadRequestResponse)message;

            context.NotifyDownloadFailed(RequestErrorConverter.Convert(response.ErrorCode), response.FileName);
        }
    }
}
