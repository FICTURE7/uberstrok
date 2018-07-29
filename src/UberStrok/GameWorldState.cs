using System;
using System.Collections.Generic;
using System.Reflection;

namespace UberStrok
{
    public abstract class GameWorldState : State
    {
        /* Game instance to which this game state is tied to. */
        internal GameWorld _game;
        /* Command filter for this game state. */
        internal readonly CommandFilter _filter;
        /* Dictionary mapping event types to their on event method in this game state. */
        internal readonly Dictionary<Type, MethodInfo> _onEventMethods;

        protected GameWorldState()
        {
            _filter = new CommandFilter();
            _onEventMethods = new Dictionary<Type, MethodInfo>();

            /* TODO: Cache the results in a static dictionary or something. */
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                /* 
                    Look for methods which has the name "OnEvent,
                    returns a void (returns no value) and has 1
                    parameter whose type inherits from Event.
                 */
                if (method.Name != "OnEvent" || method.ReturnType != typeof(void))
                    continue;

                /* Check parameter count. */
                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    continue;

                /* Check parameter return type. */
                var parameter = parameters[0];
                if (!parameter.ParameterType.IsSubclassOf(typeof(GameWorld.Event)))
                    continue;

                _onEventMethods.Add(parameter.ParameterType, method);
            }
        }

        protected CommandFilter Filter => _filter;
        protected GameWorld Game => _game;
    }
}
