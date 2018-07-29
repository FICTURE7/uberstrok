using System;
using System.Collections.Generic;

namespace UberStrok
{
    public abstract class Room
    {
        private readonly int _roomId;
        private readonly List<Actor> _actors;

        protected Room(int id)
        {
            _roomId = id;
            _actors = new List<Actor>(16);
        }

        public int Id => _roomId;

        public void Join(Actor actor)
        {
            if (actor == null)
                throw new ArgumentNullException(nameof(actor));

            actor._currentRoom = this;
            _actors.Add(actor);
        }

        public bool Leave(Actor actor)
        {
            if (actor == null)
                throw new ArgumentNullException(nameof(actor));

            /* Actor is not in this room. */
            if (actor._currentRoom != this)
                return false;

            if (!_actors.Contains(actor))
                return false;

            actor._currentRoom = null;
            return _actors.Remove(actor);
        }

        public void OnCommand(Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
        }
    }
}
