using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core.StateMachine.Communication
{
    public enum ClientCommunicationStateEnum
    {
        NONE = 0,
        INITIAL,
        REQUEST_FILE,
        WAIT,
        DISCONNECTING,
        DISCONNECTED,
    }
}
