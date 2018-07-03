namespace UberStrok
{
    public abstract class Component
    {
        protected abstract void OnUpdate();

        internal void DoUpdate()
        {
            OnUpdate();
        }
    }
}
