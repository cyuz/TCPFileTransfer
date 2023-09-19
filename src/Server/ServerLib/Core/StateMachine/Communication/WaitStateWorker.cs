using NetworkDLL.Message;
using ServerLib.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.Core.StateMachine.Communication
{
    public class WaitStateWorker : AbstractStateWorker
    {
        public WaitStateWorker(IFileServerCommunicationStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override ServerCommunicationStateEnum? Pump()
        {
            try
            {
                IMessage message = this.StateMachine.Conn.ReadMessage();

                if (message == null)
                {
                    return ServerCommunicationStateEnum.DISCONNECTING;
                }

                IServerMessageHandler handler = null;
                this.StateMachine.MessageHandlers.TryGetValue(message.MessageType, out handler);

                if (handler != null)
                {
                    handler.Handle(message, this.StateMachine.Context, this.StateMachine.Conn);
                }
            }
            catch (Exception)
            {
                /// TODO: log
            }

            return ServerCommunicationStateEnum.DISCONNECTING;
        }

        public override bool ShouldPump => true;

        public override ServerCommunicationStateEnum State => ServerCommunicationStateEnum.WAIT;
    }
}
