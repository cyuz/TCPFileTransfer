using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.Core.StateMachine.Communication
{
    public class DisconnectedStateWorker : AbstractStateWorker
    {
        public DisconnectedStateWorker(IFileServerCommunicationStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override ServerCommunicationStateEnum? Pump()
        {
            return null;
        }

        public override bool ShouldPump => false;

        public override ServerCommunicationStateEnum State => ServerCommunicationStateEnum.DISCONNECTED;
    }
}
