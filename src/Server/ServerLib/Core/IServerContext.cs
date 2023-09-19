using NetworkDLL.File;
using System;

namespace ServerLib.Core
{
    public interface IServerContext
    {
        string FileFolder { get; }
        IFileUtil FileUtil { get; }

        event Action<string, int, string, DateTime, bool> OnReceiveRequest;

        void NotifyRequestReceived(string ip, int port, string fileName, DateTime time, bool exist);
    }
}