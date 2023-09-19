using NetworkDLL.File;
using System;

namespace ClientLib.Core
{
    public interface IClientContext
    {
        string FileName { get; }
        string FilePath { get; }
        IFileUtil FileUtil { get; }

        event Action<string> OnDownloadComplete;
        event Action<DownloadErrorCode, string> OnDownloadFailed;

        void NotifyDownloadComplete(string fileName);
        void NotifyDownloadFailed(DownloadErrorCode errorCode, string desc);
    }
}