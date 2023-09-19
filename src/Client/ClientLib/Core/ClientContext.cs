using NetworkDLL.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Core
{
    public class ClientContext : IClientContext
    {
        private string _fileName;
        public string FileName => _fileName;

        private string _filePath;

        public string FilePath => _filePath;

        private IFileUtil _fileUtil;
        public IFileUtil FileUtil => _fileUtil;

        public event Action<string> OnDownloadComplete;
        public event Action<DownloadErrorCode, string> OnDownloadFailed;

        public ClientContext(IFileUtil fileUtil, string fileName, string filePath)
        {
            _fileUtil = fileUtil;
            _fileName = fileName;
            _filePath = filePath;
        }

        public void NotifyDownloadComplete(string fileName)
        {
            this.OnDownloadComplete?.Invoke(fileName);
        }

        public void NotifyDownloadFailed(DownloadErrorCode errorCode, string desc)
        {
            this.OnDownloadFailed?.Invoke(errorCode, desc);
        }
    }
}
