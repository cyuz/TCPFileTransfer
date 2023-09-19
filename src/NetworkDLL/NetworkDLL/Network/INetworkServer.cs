using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDLL.Network
{
    public interface INetworkServer
    {
        INetworkClient AcceptNetworkClient();
        void Start(int port);
        void Stop();
    }
}
