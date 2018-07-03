using System;
using System.Collections.Generic;

namespace UberStrok
{
    public class GameObject
    {
        private readonly Dictionary<Type, Component> _components;

        public GameObject()
        {
            _components = new Dictionary<Type, Component>();
        }

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
    }
}
