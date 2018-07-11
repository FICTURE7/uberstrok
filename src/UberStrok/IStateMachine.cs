namespace UberStrok
{
    public interface IStateMachine<TStateType> where TStateType : State
    {
        void ResetState();
        void RegisterState<TState>() where TState : TStateType, new();
        void SetState<TState>() where TState : TStateType, new();
        TStateType GetState();
    }
}
