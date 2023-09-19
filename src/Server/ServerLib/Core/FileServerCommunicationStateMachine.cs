using NetworkDLL.Message;
using NetworkDLL.Network;
using ServerLib.Core.StateMachine.Communication;
using ServerLib.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerLib.Core
{
    public class FileServerCommunicationStateMachine : IFileServerCommunicationStateMachine
    {
        private IServerContext _context;
        public IServerContext Context => _context;
        private INetworkConnection _conn;
        public INetworkConnection Conn => _conn;
        private IMyNetworkAdapter _myNetworkAdapter;
        public IMyNetworkAdapter MyNetworkAdapter => _myNetworkAdapter;

        private Dictionary<MessageTypeEnum, IServerMessageHandler> _messageHandlers = new Dictionary<MessageTypeEnum, IServerMessageHandler>();

        public IReadOnlyDictionary<MessageTypeEnum, IServerMessageHandler> MessageHandlers => _messageHandlers;

        private Dictionary<ServerCommunicationStateEnum, IStateWorker> _stateWorkers = new Dictionary<ServerCommunicationStateEnum, IStateWorker>();

        private IStateWorker _stateWorker = null;

        public bool ShouldPump => this._stateWorker?.ShouldPump ?? false;

        public ServerCommunicationStateEnum State => this._stateWorker?.State ?? ServerCommunicationStateEnum.NONE;

        public FileServerCommunicationStateMachine(IServerContext context, INetworkConnection conn, IMyNetworkAdapter myNetworkAdapter)
        {
            _context = context;
            _conn = conn;
            _myNetworkAdapter = myNetworkAdapter;

            _messageHandlers.Add(MessageTypeEnum.GET_FILE_REQUEST, new GetFileRequestMessageHandler(_myNetworkAdapter));

            _stateWorkers.Add(ServerCommunicationStateEnum.INITIAL, new InitialStateWorker(this));
            _stateWorkers.Add(ServerCommunicationStateEnum.WAIT, new WaitStateWorker(this));
            _stateWorkers.Add(ServerCommunicationStateEnum.DISCONNECTING, new DisconnectingStateWorker(this));
            _stateWorkers.Add(ServerCommunicationStateEnum.DISCONNECTED, new DisconnectedStateWorker(this));

            _stateWorker = _stateWorkers[ServerCommunicationStateEnum.INITIAL];
        }

        public void ChangeStateWorkers(Dictionary<ServerCommunicationStateEnum, IStateWorker> stateWorkers, ServerCommunicationStateEnum newState)
        {
            this._stateWorkers.Clear();
            this._stateWorker = null;
            foreach (var keyValuePair in stateWorkers)
            {
                this._stateWorkers.Add(keyValuePair.Key, keyValuePair.Value);
            }

            if (this._stateWorkers.ContainsKey(newState))
            {
                _stateWorker = _stateWorkers[newState];
            }
            else
            {
                throw new ArgumentException($"{newState} no state machine worker");
            }
        }

        public void Pump()
        {
            ServerCommunicationStateEnum? nextState = this._stateWorker.Pump();
            if (nextState.HasValue)
            {
                IStateWorker newWorker = null;
                if (this._stateWorkers.TryGetValue(nextState.Value, out newWorker))
                {
                    this._stateWorker = newWorker;
                }
                else
                {
                    throw new InvalidOperationException($"not registered state worker:{nextState}");
                }
            }
        }
    }
}
