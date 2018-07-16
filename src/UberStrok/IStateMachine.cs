namespace UberStrok
{
    public interface IStateMachine<TStateBase> where TStateBase : IState
    {
        void ResetState();
        TState RegisterState<TState>() where TState : TStateBase, new();
        void SetState<TState>() where TState : TStateBase, new();
        TStateBase GetState();
    }
}
