using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core.StateMachine.Communication
{
    public class DisconnectingStateWorker : AbstractStateWorker
    {
        public DisconnectingStateWorker(IFileClientCommunicationStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override ClientCommunicationStateEnum? Pump()
        {
            this.StateMachine.Conn.Disconnect();

            return ClientCommunicationStateEnum.DISCONNECTED;
        }

        public override bool ShouldPump => true;

        public override ClientCommunicationStateEnum State => ClientCommunicationStateEnum.DISCONNECTING;
    }
}
