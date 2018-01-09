namespace UberStrok.Realtime.Server
{
    public abstract class BaseRooms<T> where T : BasePeer
    {
        public T[] Peers => null;

        public abstract void OnJoin(T peer);

        public abstract void OnLeave(T peer);
    }
}
