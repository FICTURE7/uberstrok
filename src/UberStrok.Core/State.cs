namespace UberStrok.Core
{
    public abstract class State
    {
        public abstract void OnEnter();
        public abstract void OnResume();
        public abstract void OnUpdate();
        public abstract void OnExit();
    }
}
