using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.Core
{
    public enum StartFailedCode
    {
        NONE = 0,
        UNKNONW_REASON = 1,
        SOCKET_ERROR = 2,
        PORT_OCCUPIED = 3,
    }
}
