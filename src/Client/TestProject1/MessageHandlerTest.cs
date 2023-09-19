using ClientLib.Core;
using ClientLib.MessageHandler;
using Moq;
using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;
using NUnit.Framework;

namespace TestProject1
{
    public class MessageHandlerTest
    {
        Mock<INetworkConnection> _connMoq = new Mock<INetworkConnection>();
        Mock<IFileUtil> _fileUtilMoq = new Mock<IFileUtil>();
        Mock<IMyNetworkAdapter> _adapterMoq = new Mock<IMyNetworkAdapter>();
        Mock<IClientContext> _contextMoq = new Mock<IClientContext>();

        [SetUp]
        public void Setup()
        {
            _contextMoq.SetupGet(m => m.FileUtil).Returns(_fileUtilMoq.Object);
            _contextMoq.SetupGet(m => m.FileName).Returns("FileName");
            _contextMoq.SetupGet(m => m.FilePath).Returns("FilePath");
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
        public void TestGotFileReponseMessageHandler()
        {
            _fileUtilMoq.Setup(m => m.CanWrite(It.IsAny<string>())).Returns(true);

            _adapterMoq.Setup(m => m.SaveFileFromNetworkStream(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<IFileUtil>(), It.IsAny<INetworkConnection>())).Returns(true);

            var conn = _connMoq.Object;
            var context = _contextMoq.Object;
            var adapter = _adapterMoq.Object;

            GotFileResponse msg = new GotFileResponse { FileName = "abc", FileLength = 100L };

            GotFileResponseMessageHandler handler = new GotFileResponseMessageHandler(adapter);
            handler.Handle(msg, context, conn);

            _adapterMoq.Verify(m => m.SaveFileFromNetworkStream(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<IFileUtil>(), It.IsAny<INetworkConnection>()), Times.Once);

            _contextMoq.Verify(m => m.NotifyDownloadComplete(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void TestGotFileReponseMessageHandlerAndLoadFail()
        {
            _fileUtilMoq.Setup(m => m.CanWrite(It.IsAny<string>())).Returns(true);

            _adapterMoq.Setup(m => m.SaveFileFromNetworkStream(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<IFileUtil>(), It.IsAny<INetworkConnection>())).Returns(false);

            var conn = _connMoq.Object;
            var context = _contextMoq.Object;
            var adapter = _adapterMoq.Object;

            GotFileResponse msg = new GotFileResponse { FileName = "abc", FileLength = 100L };

            GotFileResponseMessageHandler handler = new GotFileResponseMessageHandler(adapter);
            handler.Handle(msg, context, conn);

            _adapterMoq.Verify(m => m.SaveFileFromNetworkStream(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<IFileUtil>(), It.IsAny<INetworkConnection>()), Times.Once);

            _contextMoq.Verify(m => m.NotifyDownloadFailed(DownloadErrorCode.DONWLOAD_ERROR, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void TestGotFileReponseMessageHandlerButCannotWrite()
        {
            _fileUtilMoq.Setup(m => m.CanWrite(It.IsAny<string>())).Returns(false);

            _adapterMoq.Setup(m => m.SaveFileFromNetworkStream(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<IFileUtil>(), It.IsAny<INetworkConnection>())).Returns(true);

            var conn = _connMoq.Object;
            var context = _contextMoq.Object;
            var adapter = _adapterMoq.Object;

            GotFileResponse msg = new GotFileResponse { FileName = "abc", FileLength = 100L };

            GotFileResponseMessageHandler handler = new GotFileResponseMessageHandler(adapter);
            handler.Handle(msg, context, conn);

            _adapterMoq.Verify(m => m.SaveFileFromNetworkStream(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<IFileUtil>(), It.IsAny<INetworkConnection>()), Times.Never);

            _contextMoq.Verify(m => m.NotifyDownloadFailed(DownloadErrorCode.CANNOT_WRITE, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void TestBadRequestReponseMessageHandler()
        {
            var conn = _connMoq.Object;
            var context = _contextMoq.Object;
            var adapter = _adapterMoq.Object;

            BadRequestResponse msg = new BadRequestResponse { FileName = "abc", ErrorCode = RequestErrorCode.FILE_NOT_FOUND };

            BadRequestResponseMessageHandler handler = new BadRequestResponseMessageHandler(adapter);
            handler.Handle(msg, context, conn);

            _contextMoq.Verify(m => m.NotifyDownloadFailed(DownloadErrorCode.FILE_NOT_FOUND, It.IsAny<string>()), Times.Once);
        }
    }
}
