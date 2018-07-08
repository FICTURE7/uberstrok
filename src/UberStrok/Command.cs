using System.Runtime.CompilerServices;

namespace UberStrok
{
    public abstract class Command
    {
        internal int _tick;
        public int Tick => _tick;

        protected abstract void OnExecute(Game game, GameObject gameObject);

        /* wrapper */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DoExecute(Game game, GameObject gameObject) => OnExecute(game, gameObject);
    }
}
