using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.Core.StateMachine.Communication
{
    public enum ServerCommunicationStateEnum
    {
        NONE = 0,
        INITIAL,
        WAIT,
        DISCONNECTING,
        DISCONNECTED,
    }
}
