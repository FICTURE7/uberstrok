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
                throw new Exception("Component already exists!");

            var component = new TComponent();
            _components.Add(type, component);
            return component;
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
            foreach(var kv in _components)
                kv.Value.DoUpdate();
        }
    }
}
