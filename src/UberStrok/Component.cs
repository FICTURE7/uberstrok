using System.Runtime.CompilerServices;

namespace UberStrok
{
    public abstract class Component
    {
        internal GameObject _gameObject;

        protected Component()
        {
            /* Space */
        }

        /* GameObject which owns this Component. */
        public GameObject GameObject => _gameObject;

        protected abstract void OnUpdate();

        /* wrapper */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DoUpdate() => OnUpdate();
    }
}
