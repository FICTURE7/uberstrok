using System;

namespace UberStrok
{
    public abstract class GameState : State
    {
        public static readonly GameState Empty = new EmptyGameState();

        public CommandFilter Commands => null;

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
