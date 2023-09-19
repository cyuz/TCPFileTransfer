using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;
using ServerLib.Core;

namespace ServerLib.MessageHandler
{
    public interface IServerMessageHandler
    {
        public void Handle(IMessage message, IServerContext context, INetworkConnection conn);
    }
}
