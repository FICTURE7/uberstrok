using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UberStrok
{
    public abstract class Room : ICommandDispatcher, IEventDispatcher
    {
        /* Tick of the current room. */
        private int _tick;
        /* List of actors in this room. */
        private readonly List<Actor> _actors;
        /* Dictionary mapping event types to their on event method in this game state. */
        private readonly Dictionary<Type, MethodInfo> _onEventMethods;

        public IReadOnlyList<Actor> Actors => _actors.AsReadOnly();

        protected Room()
        {
            _actors = new List<Actor>(16);
            _onEventMethods = Event.GetEvents(GetType());
        }

        internal void DoJoin(Actor actor)
        {
            Debug.Assert(actor != null);
            _actors.Add(actor);
            OnJoin(actor);
        }

        internal void DoLeave(Actor actor)
        {
            Debug.Assert(actor != null);
            var result = _actors.Remove(actor);
            Debug.Assert(result);
            OnLeave(actor);
        }

        public abstract void DoTick();

        protected virtual void OnJoin(Actor actor)
        {
            /* Space */
        }

        protected virtual void OnLeave(Actor actor)
        {
            /* Space */
        }

        public virtual void OnCommand(Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            command._tick = _tick;
            /* Default implementation of Room.OnCommand executes instantly. */
            command.DoExecute();
        }

        public virtual void OnEvent<TEvent>(TEvent @event) where TEvent : Event
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            /* TODO: Implement event dispatch. */
        }
    }
}
