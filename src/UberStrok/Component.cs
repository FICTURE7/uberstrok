using System.Runtime.CompilerServices;

namespace UberStrok
{
    public abstract class Component
    {
        /* GameObject which owns this Component. */
        internal GameObject _gameObject;

        public GameObject GameObject => _gameObject;

        protected abstract void OnUpdate();

        /* Wrapper. */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DoUpdate()
        {
            OnUpdate();
        }
    }
}
