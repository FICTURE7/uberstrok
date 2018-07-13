using System;
using System.Collections;
using System.Collections.Generic;

namespace UberStrok
{
    public class GameObjectCollection : IReadOnlyCollection<GameObject>
    {
        /* Game instance which owns this collection. */
        private readonly Game _game;
        /* Dictionary mapping game object names to game object instances. */
        private readonly Dictionary<string, GameObject> _objects;

        internal GameObjectCollection(Game game)
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
