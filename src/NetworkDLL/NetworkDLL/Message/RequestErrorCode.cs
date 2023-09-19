using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Message
{
    public enum RequestErrorCode
    {
        NONE = 0,
        FILE_NOT_FOUND = 1,
        CANNOT_READ = 2,
    }
}
