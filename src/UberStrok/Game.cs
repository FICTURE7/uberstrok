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

        private void DoUpdate()
        {
            for (int i = 0; i < _gameObjects.Count; i++)
                _gameObjects[i].DoUpdate();
        }
    }
}
