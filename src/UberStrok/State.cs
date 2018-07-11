namespace UberStrok
{
    public abstract class State : IState
    {
        public abstract void OnEnter();
        public abstract void OnResume();
        public abstract void OnUpdate();
        public abstract void OnExit();
    }
}
