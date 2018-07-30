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
            _onEventMethods = Event.GetEvents(GetType());
        }

        protected CommandFilter Filter => _filter;
        protected GameWorld Game => _game;
    }
}
