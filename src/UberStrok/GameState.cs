namespace UberStrok
{
    public abstract class GameState : State
    {
        /* Game instance to which this game state is tied to. */
        internal Game _game;
        /* Command filter for this game state. */
        internal readonly CommandFilter _filter;
        
        protected GameState()
        {
            _filter = new CommandFilter();
        }

        protected CommandFilter Filter => _filter;
        protected Game Game => _game;
    }
}
