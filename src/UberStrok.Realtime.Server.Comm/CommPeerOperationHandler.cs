using log4net;
using System;
using System.Text;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeerOperationHandler : BaseCommPeerOperationHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommPeerOperationHandler));

        public CommPeerOperationHandler(CommPeer peer) : base(peer)
        {
            // Space
        }

        public override void OnAuthenticationRequest(string authToken, string magicHash)
        {
            Log.Info($"Received AuthenticationRequest! {authToken}:{magicHash}");

            // Parse the auth token to retrieve the web server address.
            // TODO: Make the WebServer send its external IP instead to avoid issues
            // with custom service base strings.
            var bytes = Convert.FromBase64String(authToken);
            var data = Encoding.UTF8.GetString(bytes);

            var webServer = data.Substring(0, data.IndexOf("#####"));

            // Retrieve user data from the web server.
            var client = new UserWebServiceClient(webServer);
            var member = client.GetMember(authToken);

            var view = new CommActorInfoView
            {
                AccessLevel = member.CmuneMemberView.PublicProfile.AccessLevel,
                Channel = ChannelType.Steam,
                Cmid = member.CmuneMemberView.PublicProfile.Cmid,
                PlayerName = member.CmuneMemberView.PublicProfile.Name,
            };
            var actor = new CommActor(Peer, view);

            Peer.Actor = actor;
            // Add user to the lobby room's actors.
            LobbyManager.Instance.Add(actor);
            // Update all peers' actor list including this peer.
            LobbyManager.Instance.UpdateList();
        }

        public override void OnSendHeartbeatResponse(string authToken, string responseHash)
        {
            // Space
        }
    }
}
