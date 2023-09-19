using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core.StateMachine.Communication
{
    public class RequestFileSateWorker : AbstractStateWorker
    {
        public RequestFileSateWorker(IFileClientCommunicationStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override ClientCommunicationStateEnum? Pump()
        {
            this.StateMachine.MyNetworkAdapter.WriteGetFileRequestIntoNetworkStream(this.StateMachine.Context.FileName, this.StateMachine.Conn);

            return ClientCommunicationStateEnum.WAIT;
        }

        public override bool ShouldPump => true;

        public override ClientCommunicationStateEnum State => ClientCommunicationStateEnum.REQUEST_FILE;
    }
}
