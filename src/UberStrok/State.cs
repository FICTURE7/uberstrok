using System.Runtime.CompilerServices;

namespace UberStrok
{
    public abstract class State
    {
        protected abstract void OnEnter();
        protected abstract void OnResume();
        protected abstract void OnUpdate();
        protected abstract void OnExit();

        /* wrapper */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DoUpdate() => OnUpdate();
    }
}
