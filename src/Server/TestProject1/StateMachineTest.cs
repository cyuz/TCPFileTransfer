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

namespace TestProject1
{
    public class StateMachineTest
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
        public void NormalTest()
        {
            var conn = _connMoq.Object;
            var adapter = _adapterMoq.Object;
            var context = _contextMoq.Object;

            FileServerCommunicationStateMachine stateMachine = new FileServerCommunicationStateMachine(context, conn, adapter);

            Assert.AreEqual(ServerCommunicationStateEnum.INITIAL, stateMachine.State);
        }

        [Test]
        public void NoNextStateTest()
        {
            var conn = _connMoq.Object;
            var adapter = _adapterMoq.Object;
            var context = _contextMoq.Object;

            Dictionary<MessageTypeEnum, IServerMessageHandler> messageHandlers = new Dictionary<MessageTypeEnum, IServerMessageHandler>();
            Dictionary<ServerCommunicationStateEnum, IStateWorker> stateWorkers = new Dictionary<ServerCommunicationStateEnum, IStateWorker>();

            FileServerCommunicationStateMachine stateMachine = new FileServerCommunicationStateMachine(context, conn, adapter);

            stateWorkers.Add(ServerCommunicationStateEnum.INITIAL, new InitialStateWorker(stateMachine));

            stateMachine.ChangeStateWorkers(stateWorkers, ServerCommunicationStateEnum.INITIAL);

            Exception ex = Assert.Throws<InvalidOperationException>(() => stateMachine.Pump());
            
        }
    }
}
