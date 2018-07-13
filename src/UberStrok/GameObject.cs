using System;
using System.Collections.Generic;

namespace UberStrok
{
    public class GameObject
    {
        /* TODO: Filter DataComponents and Components into seperate dictionaries. */

        /* Components in this GameObject. */
        private readonly Dictionary<Type, Component> _components;
        /* Game instance which owns this GameObject. */
        private readonly Game _game;

        public GameObject(Game game) : this(game, null)
        {
            /* Space */
        }

        public GameObject(Game game, string name)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            /*
                Add the new game object to the
                game's list of objects.
             */
            _game = game;
            _game._objects.Add(this);

            _components = new Dictionary<Type, Component>();

            Enable = true;
            Name = name;
        }

        public bool Enable { get; set; }
        public string Name { get; set; }
        public Game Game => _game;

        public TComponent AddComponent<TComponent>() where TComponent : Component, new()
        {
            var type = typeof(TComponent);
            if (_components.ContainsKey(type))
                throw new InvalidOperationException("Component already exists.");

            var component = new TComponent();
            component._gameObject = this;
            _components.Add(type, component);
            return component;
        }

        public bool RemoveComponent<TComponent>() where TComponent : Component, new()
        {
            var type = typeof(TComponent);
            var component = GetComponent<TComponent>();

            if (component == null)
                return false;

            component._gameObject = null;
            return _components.Remove(type);
        }

        public TComponent GetComponent<TComponent>() where TComponent : Component, new()
        {
            var component = default(Component);
            _components.TryGetValue(typeof(TComponent), out component);
            return (TComponent)component;
        }

        internal void DoUpdate()
        {
            /* NOTE: Performance loss here. */
            /* TOOO: Avoid foreach. */
            foreach(var kv in _components)
                kv.Value.DoUpdate();
        }

        public override string ToString()
        {
            return $"{{{Name}:{_components.Count}}}";
        }
    }
}
