using Moq;
using NetworkDLL.File;
using NUnit.Framework;
using ServerLib.Core;
using System;

namespace TestProject1
{
    public class ServerContextTest
    {
        private bool _recvNotify;

        [SetUp]
        public void Setup()
        {
            _recvNotify = true;
        }

        [Test]
        public void TestNotifyReceiveRequest()
        {
            Mock<IFileUtil> fileUtilMoq = new Mock<IFileUtil>();
            var fileUtil = fileUtilMoq.Object;
            ServerContext context = new ServerContext(fileUtil, "abc");

            context.OnReceiveRequest += Context_OnReceiveRequest;

            Assert.IsTrue(_recvNotify);
        }

        private void Context_OnReceiveRequest(string arg1, int arg2, string arg3, DateTime arg4, bool arg5)
        {
            _recvNotify = true;
        }
    }
}
