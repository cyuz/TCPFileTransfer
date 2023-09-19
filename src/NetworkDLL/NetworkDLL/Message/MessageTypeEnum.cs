using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Message
{
    public enum MessageTypeEnum
    {
        NONE = 0,
        GET_FILE_REQUEST,
        BAD_REQUEST_RESPONSE,
        GOT_FILE_RESPONSE,
        END = GOT_FILE_RESPONSE,
    }
}
