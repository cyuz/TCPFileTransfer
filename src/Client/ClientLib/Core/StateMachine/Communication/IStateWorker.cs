namespace ClientLib.Core.StateMachine.Communication
{
    public interface IStateWorker
    {
        bool ShouldPump { get; }
        ClientCommunicationStateEnum State { get; }

        ClientCommunicationStateEnum? Pump();
    }
}