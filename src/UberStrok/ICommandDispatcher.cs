namespace UberStrok
{
    public interface ICommandDispatcher
    {
        void OnCommand(Command command);
    }
}
