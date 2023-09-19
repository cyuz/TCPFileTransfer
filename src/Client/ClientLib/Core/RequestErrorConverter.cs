using NetworkDLL.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core
{
    public static class RequestErrorConverter
    {
        public static DownloadErrorCode Convert(RequestErrorCode errorCode)
        {
            switch(errorCode)
            {
                case RequestErrorCode.FILE_NOT_FOUND:
                    return DownloadErrorCode.FILE_NOT_FOUND;
                case RequestErrorCode.CANNOT_READ:
                    return DownloadErrorCode.CANNOT_READ;
                default:
                    return DownloadErrorCode.UNKNONW_ERRO;
            }
        }
    }
}
