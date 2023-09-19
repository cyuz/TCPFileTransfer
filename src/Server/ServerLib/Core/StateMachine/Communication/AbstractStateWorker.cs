using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.Core.StateMachine.Communication
{
    public abstract class AbstractStateWorker : IStateWorker
    {
        private IFileServerCommunicationStateMachine _stateMachine;

        protected IFileServerCommunicationStateMachine StateMachine => _stateMachine;

        protected AbstractStateWorker(IFileServerCommunicationStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public abstract bool ShouldPump { get; }

        public abstract ServerCommunicationStateEnum? Pump();

        public abstract ServerCommunicationStateEnum State { get; }
    }
}
