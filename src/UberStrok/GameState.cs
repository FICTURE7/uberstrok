namespace UberStrok
{
    public abstract class GameState : State
    {
        /* Game instance to which this game state is tied to. */
        internal Game _game;
        private readonly CommandFilter _filter;
        
        protected GameState()
        {
            _filter = new CommandFilter();
        }

        public CommandFilter Filter => _filter;
        protected Game Game => _game;
    }
}
