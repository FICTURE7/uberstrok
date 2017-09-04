using log4net;
using System;
using System.Text;
using UbzStuff.Core.Common;
using UbzStuff.Core.Views;
using UbzStuff.WebServices.Client;

namespace UbzStuff.Realtime.Server.Comm
{
    public class CommPeerOperationHandler : BaseCommPeerOperationHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommPeerOperationHandler));

        public CommPeerOperationHandler(CommPeer peer) : base(peer)
        {
            // Space
        }

        public override int Id => 1;

        public override void OnAuthenticationRequest(string authToken, string magicHash)
        {
            //TODO: Use AuthToken to retrieve data from the web services.
            Log.Info($"Received AuthenticationRequest! {authToken}:{magicHash}");

            var bytes = Convert.FromBase64String(authToken);
            var data = Encoding.UTF8.GetString(bytes);

            var webServer = data.Substring(0, data.IndexOf("#####"));

            // Retrieve user data from the web server.
            var client = new UserWebServiceClient(webServer);
            var member = client.GetMember(authToken);

            var actor = new CommActorInfoView
            {
                AccessLevel = member.CmuneMemberView.PublicProfile.AccessLevel,
                Channel = ChannelType.Steam,
                Cmid = member.CmuneMemberView.PublicProfile.Cmid,
                PlayerName = member.CmuneMemberView.PublicProfile.Name,                
            };

            Peer.Actor = actor;

            // Add user to the lobby room's actors.
            LobbyRoomManager.Instance.Actors.Add(actor);

            Peer.Events.SendLobbyEntered();
            // Send all actors in the lobby to the player.
            Peer.Lobby.Events.SendFullPlayerListUpdate(LobbyRoomManager.Instance.Actors);
        }

        public override void OnSendHeartbeatResponse(string authToken, string responseHash)
        {
            // Space
        }
    }
}
