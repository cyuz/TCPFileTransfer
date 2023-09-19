using Moq;
using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;
using NUnit.Framework;
using ServerLib.Core;
using ServerLib.Core.StateMachine.Communication;
using ServerLib.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    class StateMachineWorkerTest
    {
        Mock<IFileServerCommunicationStateMachine> _stateMachineMoq = new Mock<IFileServerCommunicationStateMachine>();
        Mock<INetworkConnection> _connMoq = new Mock<INetworkConnection>();
        Mock<IFileUtil> _fileUtilMoq = new Mock<IFileUtil>();
        Mock<IMyNetworkAdapter> _adapterMoq = new Mock<IMyNetworkAdapter>();
        Mock<IServerContext> _contextMoq = new Mock<IServerContext>();
        Mock<IServerMessageHandler> _getFileMessageHandlerMoq = new Mock<IServerMessageHandler>();

        [SetUp]
        public void Setup()
        {
            _contextMoq.SetupGet(m => m.FileUtil).Returns(_fileUtilMoq.Object);
            _contextMoq.SetupGet(m => m.FileFolder).Returns("PATH");
            
            var conn = _connMoq.Object;
            var adapter = _adapterMoq.Object;
            var context = _contextMoq.Object;

            _stateMachineMoq.Setup(m => m.Conn).Returns(conn);
            _stateMachineMoq.Setup(m => m.Context).Returns(context);
            _stateMachineMoq.Setup(m => m.MyNetworkAdapter).Returns(adapter);

            _getFileMessageHandlerMoq.Setup(m => m.Handle(It.IsAny<IMessage>(), It.IsAny<ServerContext>(), It.IsAny<INetworkConnection>())).Verifiable();
        }

        [TearDown]
        public void TearDown()
        {
            _stateMachineMoq.Reset();
            _connMoq.Reset();
            _fileUtilMoq.Reset();
            _adapterMoq.Reset();
            _contextMoq.Reset();
            _getFileMessageHandlerMoq.Reset();
        }

        [Test]
        public void TestInitialState()
        {
            IStateWorker worker = new InitialStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ServerCommunicationStateEnum.INITIAL, worker.State);
            ServerCommunicationStateEnum? nextState = worker.Pump();

            Assert.AreEqual(ServerCommunicationStateEnum.WAIT, nextState);
        }

        [Test]
        public void TestWaitStateWithoutMessage()
        {
            IStateWorker worker = new WaitStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ServerCommunicationStateEnum.WAIT, worker.State);
            ServerCommunicationStateEnum? nextState = worker.Pump();

            _connMoq.Verify(m => m.ReadMessage(), Times.Once);

            _getFileMessageHandlerMoq.Verify(m => m.Handle(It.IsAny<IMessage>(), It.IsAny<ServerContext>(), It.IsAny<INetworkConnection>()), Times.Never);

            Assert.AreEqual(ServerCommunicationStateEnum.DISCONNECTING, nextState);
        }

        [Test]
        public void TestWaitStateWithoutMessageButNoHandler()
        {
            _connMoq.Setup(m => m.ReadMessage()).Returns(new GetFileRequest { FileName = "ABC" });

            IStateWorker worker = new WaitStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ServerCommunicationStateEnum.WAIT, worker.State);
            ServerCommunicationStateEnum? nextState = worker.Pump();

            _connMoq.Verify(m => m.ReadMessage(), Times.Once);

            _getFileMessageHandlerMoq.Verify(m => m.Handle(It.IsAny<IMessage>(), It.IsAny<ServerContext>(), It.IsAny<INetworkConnection>()), Times.Never);

            Assert.AreEqual(ServerCommunicationStateEnum.DISCONNECTING, nextState);
        }

        [Test]
        public void TestWaitStateWithCorrectMessageAndHandler()
        {
            _connMoq.Setup(m => m.ReadMessage()).Returns(new GetFileRequest { FileName = "ABC" });

            var handler = _getFileMessageHandlerMoq.Object;

            Dictionary<MessageTypeEnum, IServerMessageHandler> handlers = new Dictionary<MessageTypeEnum, IServerMessageHandler>();

            handlers.Add(MessageTypeEnum.GET_FILE_REQUEST, handler);

            _stateMachineMoq.Setup(m => m.MessageHandlers).Returns(handlers);

            IStateWorker worker = new WaitStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ServerCommunicationStateEnum.WAIT, worker.State);
            ServerCommunicationStateEnum? nextState = worker.Pump();

            _connMoq.Verify(m => m.ReadMessage(), Times.Once);

            _getFileMessageHandlerMoq.Verify(m => m.Handle(It.IsAny<IMessage>(), It.IsAny<IServerContext>(), It.IsAny<INetworkConnection>()), Times.Once);

            Assert.AreEqual(ServerCommunicationStateEnum.DISCONNECTING, nextState);
        }
        [Test]
        public void TestDisconnectingState()
        {
            IStateWorker worker = new DisconnectingStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(true, worker.ShouldPump);
            Assert.AreEqual(ServerCommunicationStateEnum.DISCONNECTING, worker.State);
            ServerCommunicationStateEnum? nextState = worker.Pump();

            Assert.AreEqual(ServerCommunicationStateEnum.DISCONNECTED, nextState);
        }

        [Test]
        public void TestDisconnectedState()
        {
            IStateWorker worker = new DisconnectedStateWorker(_stateMachineMoq.Object);

            Assert.AreEqual(false, worker.ShouldPump);
            Assert.AreEqual(ServerCommunicationStateEnum.DISCONNECTED, worker.State);
            ServerCommunicationStateEnum? nextState = worker.Pump();

            Assert.IsNull(nextState);
        }
    }
}
