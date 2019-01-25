using log4net;
using UberStrok.Realtime.Server.Comm.Commands;

namespace UberStrok.Realtime.Server.Comm
{
    public class LobbyRoom : Room
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CommApplication));
        /* Use a static instance, to reduce number of allocations and stress the GC less. */
        private static readonly LobbyEnteredCommand _lobbyEnteredCommand = new LobbyEnteredCommand();

        public bool IsFull => false;

        public override void DoTick()
        {
            /* TODO: Implement ticking logic. */
        }

        protected override void OnJoin(Actor actor)
        {
            var commActor = (CommActor)actor;
            _log.Info($"CommActor at {commActor.Peer.RemoteIPAddress} joined LobbyRoom.");
        }

        private void HandleAuthenticateRequest(Actor actor, AuthenticateRequestCommand command)
        {
            /* Notify peer it joined the lobby. */
            actor.Send(_lobbyEnteredCommand);
        }
    }
}
