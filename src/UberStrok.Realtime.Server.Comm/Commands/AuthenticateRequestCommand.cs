namespace UberStrok.Realtime.Server.Comm.Commands
{
    public class AuthenticateRequestCommand : CommCommand
    {
        public string AuthToken { get; set; }
        public string MagicHash { get; set; }

        protected override void OnExecute()
        {
            /* Space */
        }
    }
}