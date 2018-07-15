namespace UberStrok
{
    public interface IConnection
    {
        void SendCommand(Command command);
        Command ReceiveCommand();
    }
}
