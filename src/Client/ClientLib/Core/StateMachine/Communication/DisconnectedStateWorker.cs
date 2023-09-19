using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core.StateMachine.Communication
{
    public class DisconnectedStateWorker : AbstractStateWorker
    {
        public DisconnectedStateWorker(IFileClientCommunicationStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override ClientCommunicationStateEnum? Pump()
        {
            return null;
        }

        public override bool ShouldPump => false;

        public override ClientCommunicationStateEnum State => ClientCommunicationStateEnum.DISCONNECTED;
    }
}
