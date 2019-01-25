using System;
using System.Collections;
using System.Collections.Generic;

namespace UberStrok
{
    public class GameObjectCollection : IReadOnlyCollection<GameObject>
    {
        /* TODO: Seperate enabled game objects and disabled game objects into seperate lists. */

        /* Game instance which owns this collection. */
        private readonly GameWorld _game;
        /* Dictionary mapping game object names to game object instances. */
        private readonly Dictionary<string, GameObject> _objects;

        internal GameObjectCollection(GameWorld game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            _objects = new Dictionary<string, GameObject>();
            _game = game;
        }

        public int Count => _objects.Count;

        public GameObject Create()
        {
            return Create(null);
        }

        public GameObject Create(string name)
        {
            if (name != null && _objects.ContainsKey(name))
                throw new ArgumentException("Already contain a game object with the same name.");

            /* Assign a unique name, if name is null. */
            if (name == null)
            {
                var count = _objects.Count;
                do
                {
                    name = "object_" + _objects.Count;
                    count++;
                } while (_objects.ContainsKey(name));
            }

            /* Initialize the new game object. */
            var gameObject = new GameObject();
            gameObject._game = _game;
            gameObject.Name = name;

            _objects.Add(name, gameObject);

            return gameObject;
        }

        public bool Destroy(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));
            if (gameObject.Game != _game)
                throw new ArgumentException("Game object is in another game instance.");

            return Destroy(gameObject.Name);
        }

        public bool Destroy(string name)
        {
            return _objects.Remove(name);
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return _objects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
