using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.Core
{
    public interface IFileServerProxy
    {
        public event Action OnStart;
        public event Action<StartFailedCode> OnStartFailed;
        public event Action OnStop;
        public event Action<string, int, string, DateTime, bool> OnReceiveRequest;

        public void Start(int port, string fileFolder);

        public void Stop();
    }
}
