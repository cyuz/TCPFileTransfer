using ClientLib.Core;
using Moq;
using NetworkDLL.File;
using NUnit.Framework;
using System;

namespace TestProject1
{
    public class ClientContextTest
    {
        private bool _recvNotify;

        [SetUp]
        public void Setup()
        {
            _recvNotify = true;
        }

        [Test]
        public void TestNotifyDownloadComplete()
        {
            Mock<IFileUtil> fileUtilMoq = new Mock<IFileUtil>();
            var fileUtil = fileUtilMoq.Object;
            ClientContext context = new ClientContext(fileUtil, "abc", "abcd");

            context.OnDownloadComplete += Context_OnDownloadComplete;

            Assert.IsTrue(_recvNotify);
        }

        [Test]
        public void TestNotifyDownloadFailed()
        {
            Mock<IFileUtil> fileUtilMoq = new Mock<IFileUtil>();
            var fileUtil = fileUtilMoq.Object;
            ClientContext context = new ClientContext(fileUtil, "abc", "abcd");

            context.OnDownloadFailed += Context_OnDownloadFailed;

            Assert.IsTrue(_recvNotify);
        }

        private void Context_OnDownloadFailed(DownloadErrorCode arg1, string arg2)
        {
            _recvNotify = true;
        }

        private void Context_OnDownloadComplete(string obj)
        {
            _recvNotify = true;
        }
    }
}
