namespace UberStrok
{
    public abstract class Component
    {
        internal GameObject _gameObject;

        public GameObject GameObject => _gameObject;

        protected abstract void OnUpdate();

        /* wrapper */
        internal void DoUpdate() => OnUpdate();
    }
}
