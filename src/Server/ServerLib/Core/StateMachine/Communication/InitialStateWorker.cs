using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.Core.StateMachine.Communication
{
    public class InitialStateWorker : AbstractStateWorker
    {
        public InitialStateWorker(IFileServerCommunicationStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override ServerCommunicationStateEnum? Pump()
        {
            return ServerCommunicationStateEnum.WAIT;
        }

        public override bool ShouldPump => true;

        public override ServerCommunicationStateEnum State => ServerCommunicationStateEnum.INITIAL;
    }
}
