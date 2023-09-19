using ClientLib.Core;
using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;

namespace ClientLib.MessageHandler
{
    public interface IClientMessageHandler
    {
        public void Handle(IMessage message, IClientContext context, INetworkConnection conn);            
    }
}
