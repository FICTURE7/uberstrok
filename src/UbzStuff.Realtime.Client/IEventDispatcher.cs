namespace UbzStuff.Realtime.Client
{
    public interface IEventDispatcher
    {
        void OnEvent(byte opCode, byte[] data);
    }
}
