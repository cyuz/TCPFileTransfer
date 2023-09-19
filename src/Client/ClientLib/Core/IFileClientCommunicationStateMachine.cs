using ClientLib.Core.StateMachine.Communication;
using ClientLib.MessageHandler;
using NetworkDLL.Message;
using NetworkDLL.Network;
using System.Collections.Generic;

namespace ClientLib.Core
{
    public interface IFileClientCommunicationStateMachine
    {
        INetworkConnection Conn { get; }
        IClientContext Context { get; }
        IReadOnlyDictionary<MessageTypeEnum, IClientMessageHandler> MessageHandlers { get; }
        IMyNetworkAdapter MyNetworkAdapter { get; }
        bool ShouldPump { get; }
        public ClientCommunicationStateEnum State { get; }
        void Pump();
    }
}