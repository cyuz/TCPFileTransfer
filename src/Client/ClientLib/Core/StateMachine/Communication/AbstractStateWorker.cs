using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core.StateMachine.Communication
{
    public abstract class AbstractStateWorker : IStateWorker
    {
        private IFileClientCommunicationStateMachine _stateMachine;

        protected IFileClientCommunicationStateMachine StateMachine => _stateMachine;

        protected AbstractStateWorker(IFileClientCommunicationStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public abstract bool ShouldPump { get; }

        public abstract ClientCommunicationStateEnum? Pump();

        public abstract ClientCommunicationStateEnum State { get; }
    }
}
