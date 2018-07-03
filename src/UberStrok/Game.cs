using System.Collections.Generic;

namespace UberStrok
{
    public class Game
    {
        /* List of game objects in the game instance. */
        private readonly List<GameObject> _gameObjects;

        public Game()
        {
            _gameObjects = new List<GameObject>();
        }

        public void Tick()
        {
            DoDispatch();
            DoUpdate();
        }

        private void DoDispatch()
        {

        }

        private void DoUpdate()
        {
            for (int i = 0; i < _gameObjects.Count; i++)
                _gameObjects[i].DoUpdate();
        }
    }
}
