namespace UberStrok
{
    public interface IStateMachine<TStateBase> where TStateBase : IState
    {
        void ResetState();
        void RegisterState<TState>() where TState : TStateBase, new();
        void SetState<TState>() where TState : TStateBase, new();
        TStateBase GetState();
    }
}
