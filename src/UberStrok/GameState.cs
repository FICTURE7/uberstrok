using System;

namespace UberStrok
{
    public abstract class GameState : State
    {
        public static readonly GameState Empty = new EmptyGameState();

        /* Game instance to which this game state is tied to. */
        internal Game _game;

        public CommandFilter Filter => null;
        protected Game Game => _game;

        private class EmptyGameState : GameState
        {
            protected override void OnEnter()
            {
                // Space
            }

            protected override void OnExit()
            {
                // Space
            }

            protected override void OnResume()
            {
                // Space
            }

            protected override void OnUpdate()
            {
                // Space
            }
        }
    }
}
