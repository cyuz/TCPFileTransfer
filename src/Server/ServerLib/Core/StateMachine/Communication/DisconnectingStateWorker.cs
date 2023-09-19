using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.Core.StateMachine.Communication
{
    public class DisconnectingStateWorker : AbstractStateWorker
    {
        public DisconnectingStateWorker(IFileServerCommunicationStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override ServerCommunicationStateEnum? Pump()
        {
            this.StateMachine.Conn.Disconnect();

            return ServerCommunicationStateEnum.DISCONNECTED;
        }

        public override bool ShouldPump => true;

        public override ServerCommunicationStateEnum State => ServerCommunicationStateEnum.DISCONNECTING;
    }
}
