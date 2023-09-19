using ClientLib.Core;
using ClientLib.Core.StateMachine.Communication;
using ClientLib.MessageHandler;
using Moq;
using NetworkDLL.File;
using NetworkDLL.Message;
using NetworkDLL.Network;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace TestProject1
{
    public class StateMachineTest
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
        public void NormalTest()
        {
            var conn = _connMoq.Object;
            var context = _contextMoq.Object;
            var adapter = _adapterMoq.Object;

            FileClientCommunicationStateMachine stateMachine = new FileClientCommunicationStateMachine(context, conn, adapter);

            Assert.AreEqual(ClientCommunicationStateEnum.INITIAL, stateMachine.State);
        }

        [Test]
        public void NoNextStateTest()
        {
            var conn = _connMoq.Object;
            var context = _contextMoq.Object;
            var adapter = _adapterMoq.Object;

            Dictionary<MessageTypeEnum, IClientMessageHandler> messageHandlers = new Dictionary<MessageTypeEnum, IClientMessageHandler>();
            Dictionary<ClientCommunicationStateEnum, IStateWorker> stateWorkers = new Dictionary<ClientCommunicationStateEnum, IStateWorker>();

            FileClientCommunicationStateMachine stateMachine = new FileClientCommunicationStateMachine(context, conn, adapter);

            stateWorkers.Add(ClientCommunicationStateEnum.INITIAL, new InitialStateWorker(stateMachine));

            stateMachine.ChangeStateWorkers(stateWorkers, ClientCommunicationStateEnum.INITIAL);

            Exception ex = Assert.Throws<InvalidOperationException>(() => stateMachine.Pump());
            
        }
    }
}
