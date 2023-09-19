using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core
{
    public enum ConnectFailErrorCode
    {
        NONE = 0,
        UNKNONW_REASON = 1,
        SOCKET_ERROR = 2,
        ARGUMENT_ERROR = 3,
        CONNECTION_REFUSE = 4,
        HOST_NOT_FOUND = 5,

    }
}
