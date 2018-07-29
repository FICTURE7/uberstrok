namespace UberStrok
{
    public abstract class Actor
    {
        /* Current room in which the actor is in */
        internal Room _currentRoom;

        protected Actor()
        {
            /* Space */
        }

        public Room Room => _currentRoom;
    }
}
