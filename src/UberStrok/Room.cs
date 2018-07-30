using System;
using System.Collections.Generic;
using System.Reflection;

namespace UberStrok
{
    public abstract class Room : ICommandDispatcher, IEventDispatcher
    {
        /* The room's ID */
        private readonly int _roomId;
        /* List of actors in this room instance */
        private readonly List<Actor> _actors;
        /* Dictionary mapping event types to their on event method in this game state. */
        private readonly Dictionary<Type, MethodInfo> _onEventMethods;

        protected Room(int id)
        {
            _roomId = id;
            _actors = new List<Actor>(16);
            _onEventMethods = Event.GetEvents(GetType());
        }

        public int Id => _roomId;

        public void Join(Actor actor)
        {
            if (actor == null)
                throw new ArgumentNullException(nameof(actor));

            /* TODO: Check uniqueness of elements */
            actor._currentRoom = this;
            _actors.Add(actor);
            OnJoin(actor);
        }

        public bool Leave(Actor actor)
        {
            if (actor == null)
                throw new ArgumentNullException(nameof(actor));

            /* Actor is not in this room. */
            if (actor._currentRoom != this)
                return false;

            /* Check if actor is in the room's list */
            if (!_actors.Contains(actor))
            {
                /* wtf? should never happen since _currentRoom was equal to this intance */
                return false;
            }

            actor._currentRoom = null;
            var result = _actors.Remove(actor);
            OnLeave(actor);
            return result;
        }

        protected virtual void OnJoin(Actor actor)
        {
            /* Space */
        }

        protected virtual void OnLeave(Actor actor)
        {
            /* Space */
        }

        public void OnCommand(Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            /* Execute instantly cause its a Room. */
            command.DoExecute();
        }

        public void OnEvent<TEvent>(TEvent @event) where TEvent : Event
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
        }
    }
}
