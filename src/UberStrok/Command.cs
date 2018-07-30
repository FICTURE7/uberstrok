using System.Runtime.CompilerServices;

namespace UberStrok
{
    public abstract class Command
    {
        /* Game which has executed the command. */
        internal GameWorld _game;
        /* Tick at which the command was enqueued in a Game's command queue. */
        internal int _tick;

        public GameWorld Game => _game;
        public int Tick => _tick;

        protected abstract void OnExecute();

        /* wrapper */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DoExecute() => OnExecute();
    }
}
