using ClientLib.Core.StateMachine.Communication;
using ClientLib.MessageHandler;
using NetworkDLL.Message;
using NetworkDLL.Network;
using System;
using System.Collections.Generic;

namespace ClientLib.Core
{
    public class FileClientCommunicationStateMachine : IFileClientCommunicationStateMachine
    {
        private IClientContext _context;
        public IClientContext Context => _context;
        private INetworkConnection _conn;
        public INetworkConnection Conn => _conn;
        private IMyNetworkAdapter _myNetworkAdapter;
        public IMyNetworkAdapter MyNetworkAdapter => _myNetworkAdapter;

        private Dictionary<MessageTypeEnum, IClientMessageHandler> _messageHandlers = new Dictionary<MessageTypeEnum, IClientMessageHandler>();

        public IReadOnlyDictionary<MessageTypeEnum, IClientMessageHandler> MessageHandlers => _messageHandlers;

        private Dictionary<ClientCommunicationStateEnum, IStateWorker> _stateWorkers = new Dictionary<ClientCommunicationStateEnum, IStateWorker>();

        private IStateWorker _stateWorker = null;

        public bool ShouldPump => this._stateWorker?.ShouldPump ?? false;

        public ClientCommunicationStateEnum State => this._stateWorker?.State ?? ClientCommunicationStateEnum.NONE;

        public FileClientCommunicationStateMachine(IClientContext context, INetworkConnection conn, IMyNetworkAdapter myNetworkAdapter)
        {
            _context = context;
            _conn = conn;
            _myNetworkAdapter = myNetworkAdapter;

            _messageHandlers.Add(MessageTypeEnum.BAD_REQUEST_RESPONSE, new BadRequestResponseMessageHandler(_myNetworkAdapter));
            _messageHandlers.Add(MessageTypeEnum.GOT_FILE_RESPONSE, new GotFileResponseMessageHandler(_myNetworkAdapter));

            _stateWorkers.Add(ClientCommunicationStateEnum.INITIAL, new InitialStateWorker(this));
            _stateWorkers.Add(ClientCommunicationStateEnum.REQUEST_FILE, new RequestFileSateWorker(this));
            _stateWorkers.Add(ClientCommunicationStateEnum.WAIT, new WaitStateWorker(this));
            _stateWorkers.Add(ClientCommunicationStateEnum.DISCONNECTING, new DisconnectingStateWorker(this));
            _stateWorkers.Add(ClientCommunicationStateEnum.DISCONNECTED, new DisconnectedStateWorker(this));

            _stateWorker = _stateWorkers[ClientCommunicationStateEnum.INITIAL];
        }

        public void ChangeStateWorkers(Dictionary<ClientCommunicationStateEnum, IStateWorker> stateWorkers, ClientCommunicationStateEnum newState)
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
            ClientCommunicationStateEnum? nextState = this._stateWorker.Pump();
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
