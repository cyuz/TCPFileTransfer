namespace ServerLib.Core.StateMachine.Communication
{
    public interface IStateWorker
    {
        bool ShouldPump { get; }
        ServerCommunicationStateEnum State { get; }

        ServerCommunicationStateEnum? Pump();
    }
}