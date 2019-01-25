using System.Runtime.CompilerServices;

namespace UberStrok
{
    public abstract class Command
    {
        /* Tick at which the command was enqueued in a Game's command queue. */
        internal int _tick;
        public int Tick => _tick;

        protected abstract void OnExecute();

        /* Wrapper. */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DoExecute() => OnExecute();
    }
}
