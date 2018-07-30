namespace UberStrok
{
    public interface IEventDispatcher
    {
        void OnEvent<TEvent>(TEvent @event) where TEvent : Event;
    }
}
