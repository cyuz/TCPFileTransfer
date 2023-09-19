using Moq;
using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;
using NUnit.Framework;
using ServerLib.Core;
using ServerLib.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    public class MessageHandlerTest
    {
        Mock<INetworkConnection> _connMoq = new Mock<INetworkConnection>();
        Mock<IFileUtil> _fileUtilMoq = new Mock<IFileUtil>();
        Mock<IMyNetworkAdapter> _adapterMoq = new Mock<IMyNetworkAdapter>();
        Mock<IServerContext> _contextMoq = new Mock<IServerContext>();

        [SetUp]
        public void Setup()
        {
            _contextMoq.SetupGet(m => m.FileUtil).Returns(_fileUtilMoq.Object);
            _contextMoq.SetupGet(m => m.FileFolder).Returns("PATH");
        }

        [TearDown]
        public void TearDown()
        {
            _connMoq.Reset();
            _fileUtilMoq.Reset();
            _adapterMoq.Reset();
            _contextMoq.Reset();
        }

        [Test]
        public void TestGetFileRequestMessageHandler()
        {
            var conn = _connMoq.Object;

            _fileUtilMoq.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
            _fileUtilMoq.Setup(m => m.CanRead(It.IsAny<string>())).Returns(true);

            var adapter = _adapterMoq.Object;
            var context = _contextMoq.Object;

            GetFileRequest msg = new GetFileRequest { FileName = "abc" };

            GetFileRequestMessageHandler handler = new GetFileRequestMessageHandler(adapter);
            handler.Handle(msg, context, conn);

            _contextMoq.Verify(m => m.NotifyRequestReceived(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), true), Times.Once);

            _adapterMoq.Verify(m => m.WriteGotFileResponseIntoNetworkStream(It.IsAny<string>(), It.IsAny<IFileUtil>(), It.IsAny<INetworkConnection>()), Times.Once);

            _adapterMoq.Verify(m => m.LoadFileIntoNetworkStream(It.IsAny<string>(), It.IsAny<IFileUtil>(), It.IsAny<INetworkConnection>()), Times.Once);
        }

        [Test]
        public void TestGetFileRequestMessageHandlerNotFound()
        {
            var conn = _connMoq.Object;

            _fileUtilMoq.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            _fileUtilMoq.Setup(m => m.CanRead(It.IsAny<string>())).Returns(true);

            var adapter = _adapterMoq.Object;
            var context = _contextMoq.Object;

            GetFileRequest msg = new GetFileRequest { FileName = "abc" };

            GetFileRequestMessageHandler handler = new GetFileRequestMessageHandler(adapter);
            handler.Handle(msg, context, conn);

            _contextMoq.Verify(m => m.NotifyRequestReceived(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), false), Times.Once);

            _adapterMoq.Verify(m => m.WriteFileNotFoundResponseIntoNetworkStream(It.IsAny<string>(), It.IsAny<INetworkConnection>()), Times.Once);

        }

        [Test]
        public void TestGetFileRequestMessageHandlerCannotRead()
        {
            var conn = _connMoq.Object;

            _fileUtilMoq.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
            _fileUtilMoq.Setup(m => m.CanRead(It.IsAny<string>())).Returns(false);

            var adapter = _adapterMoq.Object;
            var context = _contextMoq.Object;

            GetFileRequest msg = new GetFileRequest { FileName = "abc" };

            GetFileRequestMessageHandler handler = new GetFileRequestMessageHandler(adapter);
            handler.Handle(msg, context, conn);

            _contextMoq.Verify(m => m.NotifyRequestReceived(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), true), Times.Once);

            _adapterMoq.Verify(m => m.WriteFileCannotReadResponseIntoNetworkStream(It.IsAny<string>(), It.IsAny<INetworkConnection>()), Times.Once);

        }
    }
}
