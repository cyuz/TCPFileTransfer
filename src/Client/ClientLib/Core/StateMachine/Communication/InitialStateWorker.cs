using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core.StateMachine.Communication
{
    public class InitialStateWorker : AbstractStateWorker
    {
        public InitialStateWorker(IFileClientCommunicationStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override ClientCommunicationStateEnum? Pump()
        {
            return ClientCommunicationStateEnum.REQUEST_FILE;
        }

        public override bool ShouldPump => true;

        public override ClientCommunicationStateEnum State => ClientCommunicationStateEnum.INITIAL;
    }
}
