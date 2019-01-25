namespace UberStrok.Realtime.Server.Game
{
    public class GameRoom : Room
    {
        private readonly GameWorld _world;
        private readonly int _id;

        public int Id => _id;
        public GameWorld World => _world;

        public GameRoom(int id)
        {
            _id = id;
        }

        public override void DoTick()
        {
            /* Space */
        }
    }
}
