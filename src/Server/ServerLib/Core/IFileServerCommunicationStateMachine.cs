using NetworkDLL.Message;
using NetworkDLL.Network;
using ServerLib.Core.StateMachine.Communication;
using ServerLib.MessageHandler;
using System.Collections.Generic;

namespace ServerLib.Core
{
    public interface IFileServerCommunicationStateMachine
    {
        INetworkConnection Conn { get; }
        IServerContext Context { get; }
        IReadOnlyDictionary<MessageTypeEnum, IServerMessageHandler> MessageHandlers { get; }
        IMyNetworkAdapter MyNetworkAdapter { get; }
        bool ShouldPump { get; }
        ServerCommunicationStateEnum State { get; }
        void Pump();
    }
}