using ClientLib.MessageHandler;
using NetworkDLL.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core.StateMachine.Communication
{
    public class WaitStateWorker : AbstractStateWorker
    {
        public WaitStateWorker(IFileClientCommunicationStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override ClientCommunicationStateEnum? Pump()
        {
            try
            {
                IMessage message = this.StateMachine.Conn.ReadMessage();

                if (message == null)
                {
                    this.StateMachine.Context.NotifyDownloadFailed(DownloadErrorCode.COMMUNICATE_ERROR, string.Empty);
                    return ClientCommunicationStateEnum.DISCONNECTING;
                }

                IClientMessageHandler handler = null;
                this.StateMachine.MessageHandlers.TryGetValue(message.MessageType, out handler);

                if (handler != null)
                {
                    handler.Handle(message, this.StateMachine.Context, this.StateMachine.Conn);
                }
                else
                {
                    this.StateMachine.Context.NotifyDownloadFailed(DownloadErrorCode.COMMUNICATE_ERROR, string.Empty);
                }
            }
            catch (Exception)
            {
                this.StateMachine.Context.NotifyDownloadFailed(DownloadErrorCode.COMMUNICATE_ERROR, string.Empty);
                /// TODO: log
            }

            return ClientCommunicationStateEnum.DISCONNECTING;
        }

        public override bool ShouldPump => true;

        public override ClientCommunicationStateEnum State => ClientCommunicationStateEnum.WAIT;
    }
}
