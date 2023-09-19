using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core
{
    public enum DownloadErrorCode
    {
        NONE = 0,
        UNKNONW_ERRO,
        FILE_NOT_FOUND,
        CANNOT_READ,
        DONWLOAD_ERROR,
        COMMUNICATE_ERROR,
        CANNOT_WRITE,

    }
}
