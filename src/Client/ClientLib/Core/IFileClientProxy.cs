using System;

namespace ClientLib.Core
{
    public interface IFileClientProxy
    {
        public event Action OnStart;
        public event Action OnConnectSuccess;
        public event Action<ConnectFailErrorCode> OnConnectFailed;
        public event Action OnStop;
        public event Action<string> OnDownloadComplete;
        public event Action<DownloadErrorCode, string> OnDownloadFailed;

        public void RequestFile(string ip, int port, string fileFolder, string fileName);

        public void Cancel();
    }
}
