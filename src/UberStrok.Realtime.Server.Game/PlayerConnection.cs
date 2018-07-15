namespace UberStrok.Realtime.Server.Game
{
    public class PlayerConnection : Component, IConnection
    {
        public Command ReceiveCommand()
        {
            return null;
        }

        public void SendCommand(Command command)
        {
            /* Space */
        }

        protected override void OnUpdate()
        {
            /* Space */
        }
    }
}
