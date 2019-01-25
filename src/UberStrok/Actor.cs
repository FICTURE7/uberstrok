using System;

namespace UberStrok
{
    public abstract class Actor
    {
        /* Current room the Actor is in. */
        internal Room _room;

        public Room Room => _room;

        public void Join(Room room)
        {
            _room = room ?? throw new ArgumentNullException(nameof(room));
            _room.DoJoin(this);
        }

        public void Leave()
        {
            _room.DoLeave(this);
            _room = null;
        }

        public abstract void Send(Command command);
        public abstract Command Receive();
    }
}
