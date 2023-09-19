using ClientLib.Core;
using ClientLib.Core.StateMachine.Communication;
using ClientLib.MessageHandler;
using Moq;
using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;
using NUnit.Framework;
using System.Collections.Generic;

namespace TestProject1
{
    class StateMachineWorkerTest
    {
        Mock<IFileClientCommunicationStateMachine> _stateMachineMoq = new Mock<IFileClientCommunicationStateMachine>();
        Mock<INetworkConnection> _connMoq = new Mock<INetworkConnection>();
        Mock<IFileUtil> _fileUtilMoq = new Mock<IFileUtil>();
        Mock<IMyNetworkAdapter> _adapterMoq = new Mock<IMyNetworkAdapter>();
        Mock<IClientContext> _contextMoq = new Mock<IClientContext>();
        Mock<IClientMessageHandler> _badRequestResponseMessageHandlerMoq = new Mock<IClientMessageHandler>();
        Mock<IClientMessageHandler> _gotFileResponseMessageHandlerMoq = new Mock<IClientMessageHandler>();

        [SetUp]
        public void Setup()
        {
            _contextMoq.SetupGet(m => m.FileUtil).Returns(_fileUtilMoq.Object);
            _contextMoq.SetupGet(m => m.FileName).Returns("FileName");
            _contextMoq.SetupGet(m => m.FilePath).Returns("FilePath");

            var conn = _connMoq.Object;
            var context = _contextMoq.Object;

            var adapter = _adapterMoq.Object;

            _stateMachineMoq.Setup(m => m.Conn).Returns(conn);
            _stateMachineMoq.Setup(m => m.Context).Returns(context);
            _stateMachineMoq.Setup(m => m.MyNetworkAdapter).Returns(adapter);
        }

        [TearDown]
        public void TearDown()
        {
            _stateMachineMoq.Reset();
            _connMoq.Reset();
            _fileUtilMoq.Reset();
            _adapterMoq.Reset();
            _contextMoq.Reset();
            _badRequestResponseMessageHandlerMoq.Reset();
            _gotFileResponseMessageHandlerMoq.Reset();
        }

        [Test]
        public void TestInitialState()
        {
            IStateWorker worker = new InitialStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ClientCommunicationStateEnum.INITIAL, worker.State);
            ClientCommunicationStateEnum? nextState = worker.Pump();

            Assert.AreEqual(ClientCommunicationStateEnum.REQUEST_FILE, nextState);
        }

        [Test]
        public void TestRequestFileState()
        {
            IStateWorker worker = new RequestFileSateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ClientCommunicationStateEnum.REQUEST_FILE, worker.State);
            ClientCommunicationStateEnum? nextState = worker.Pump();

            _adapterMoq.Verify(m => m.WriteGetFileRequestIntoNetworkStream(It.IsAny<string>(), It.IsAny<INetworkConnection>()), Times.Once);

            Assert.AreEqual(ClientCommunicationStateEnum.WAIT, nextState);
        }

        [Test]
        public void TestWaitStateWithoutMessage()
        {
            IStateWorker worker = new WaitStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ClientCommunicationStateEnum.WAIT, worker.State);
            ClientCommunicationStateEnum? nextState = worker.Pump();

            _connMoq.Verify(m => m.ReadMessage(), Times.Once);

            _badRequestResponseMessageHandlerMoq.Verify(m => m.Handle(It.IsAny<IMessage>(), It.IsAny<IClientContext>(), It.IsAny<INetworkConnection>()), Times.Never);
            _gotFileResponseMessageHandlerMoq.Verify(m => m.Handle(It.IsAny<IMessage>(), It.IsAny<IClientContext>(), It.IsAny<INetworkConnection>()), Times.Never);

            Assert.AreEqual(ClientCommunicationStateEnum.DISCONNECTING, nextState);
        }

        [Test]
        public void TestWaitStateWithoutMessageButNoHandler()
        {
            _connMoq.Setup(m => m.ReadMessage()).Returns(new GetFileRequest { FileName = "ABC" });

            IStateWorker worker = new WaitStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ClientCommunicationStateEnum.WAIT, worker.State);
            ClientCommunicationStateEnum? nextState = worker.Pump();

            _connMoq.Verify(m => m.ReadMessage(), Times.Once);

            _badRequestResponseMessageHandlerMoq.Verify(m => m.Handle(It.IsAny<IMessage>(), It.IsAny<IClientContext>(), It.IsAny<INetworkConnection>()), Times.Never);
            _gotFileResponseMessageHandlerMoq.Verify(m => m.Handle(It.IsAny<IMessage>(), It.IsAny<IClientContext>(), It.IsAny<INetworkConnection>()), Times.Never);

            Assert.AreEqual(ClientCommunicationStateEnum.DISCONNECTING, nextState);
        }

        [Test]
        public void TestWaitStateWithGotFileResponse()
        {
            _connMoq.Setup(m => m.ReadMessage()).Returns(new GotFileResponse { });

            Dictionary<MessageTypeEnum, IClientMessageHandler> handlers = new Dictionary<MessageTypeEnum, IClientMessageHandler>();

            handlers.Add(MessageTypeEnum.GOT_FILE_RESPONSE, _gotFileResponseMessageHandlerMoq.Object);

            _stateMachineMoq.Setup(m => m.MessageHandlers).Returns(handlers);

            IStateWorker worker = new WaitStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ClientCommunicationStateEnum.WAIT, worker.State);
            ClientCommunicationStateEnum? nextState = worker.Pump();

            _connMoq.Verify(m => m.ReadMessage(), Times.Once);

            _gotFileResponseMessageHandlerMoq.Verify(m => m.Handle(It.IsAny<IMessage>(), It.IsAny<IClientContext>(), It.IsAny<INetworkConnection>()), Times.Once);

            Assert.AreEqual(ClientCommunicationStateEnum.DISCONNECTING, nextState);
        }

        [Test]
        public void TestWaitStateWithBadRequestResponse()
        {
            _connMoq.Setup(m => m.ReadMessage()).Returns(new BadRequestResponse { });

            Dictionary<MessageTypeEnum, IClientMessageHandler> handlers = new Dictionary<MessageTypeEnum, IClientMessageHandler>();

            handlers.Add(MessageTypeEnum.BAD_REQUEST_RESPONSE, _badRequestResponseMessageHandlerMoq.Object);

            _stateMachineMoq.Setup(m => m.MessageHandlers).Returns(handlers);

            IStateWorker worker = new WaitStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ClientCommunicationStateEnum.WAIT, worker.State);
            ClientCommunicationStateEnum? nextState = worker.Pump();

            _connMoq.Verify(m => m.ReadMessage(), Times.Once);

            _badRequestResponseMessageHandlerMoq.Verify(m => m.Handle(It.IsAny<IMessage>(), It.IsAny<IClientContext>(), It.IsAny<INetworkConnection>()), Times.Once);

            Assert.AreEqual(ClientCommunicationStateEnum.DISCONNECTING, nextState);
        }

        [Test]
        public void TestDisconnectingState()
        {
            IStateWorker worker = new DisconnectingStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ClientCommunicationStateEnum.DISCONNECTING, worker.State);
            ClientCommunicationStateEnum? nextState = worker.Pump();

            Assert.AreEqual(ClientCommunicationStateEnum.DISCONNECTED, nextState);
        }

        [Test]
        public void TestDisconnectedState()
        {
            IStateWorker worker = new DisconnectedStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(false, worker.ShouldPump);
            Assert.AreEqual(ClientCommunicationStateEnum.DISCONNECTED, worker.State);
            ClientCommunicationStateEnum? nextState = worker.Pump();

            Assert.IsNull(nextState);
        }
    }
}
