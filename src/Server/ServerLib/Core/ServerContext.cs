using NetworkDLL.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.Core
{
    public class ServerContext : IServerContext
    {
        private IFileUtil _fileUtil;
        public IFileUtil FileUtil => _fileUtil;

        private string _fileFolder;
        public string FileFolder => _fileFolder;

        public event Action<string, int, string, DateTime, bool> OnReceiveRequest;

        public ServerContext(IFileUtil fileUtil, string fileFolder)
        {
            _fileUtil = fileUtil;
            _fileFolder = fileFolder;
        }

        public void NotifyRequestReceived(string ip, int port, string fileName, DateTime time, bool exist)
        {
            this.OnReceiveRequest?.Invoke(ip, port, fileName, time, exist);
        }
    }
}
