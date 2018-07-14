using System.Runtime.CompilerServices;

namespace UberStrok
{
    public abstract class Command
    {
        /* Game which has executed the command. */
        internal Game _game;
        /* Game object which issued this command. */
        internal GameObject _object;
        /* Tick at which the command was enqueued in a Game's command queue. */
        internal int _tick;

        public Game Game => _game;
        public GameObject Object => _object;
        public int Tick => _tick;

        protected abstract void OnExecute();

        /* wrapper */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DoExecute() => OnExecute();
    }
}
