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

        public CommPeerOperationHandler()
        {
            // Space
        }

        public override void OnAuthenticationRequest(CommPeer peer, string authToken, string magicHash)
        {
            Log.Info($"Received AuthenticationRequest! {authToken}:{magicHash}");

            var member = GetMemberFromAuthToken(authToken);
            var view = new CommActorInfoView
            {
                AccessLevel = member.CmuneMemberView.PublicProfile.AccessLevel,
                Channel = ChannelType.Steam,
                Cmid = member.CmuneMemberView.PublicProfile.Cmid,
                PlayerName = member.CmuneMemberView.PublicProfile.Name,
            };

            peer.Actor = new CommActor(peer, view);

            /* Make peer join the global lobby room. */
            CommApplication.Instance.Rooms.Global.Join(peer);
        }

        public override void OnSendHeartbeatResponse(CommPeer peer, string authToken, string responseHash)
        {
            // Space
        }

        private UberstrikeUserView GetMemberFromAuthToken(string authToken)
        {
            //TODO: Provide some base class for this kind of server-server communications.
            var bytes = Convert.FromBase64String(authToken);
            var data = Encoding.UTF8.GetString(bytes);

            var webServer = data.Substring(0, data.IndexOf("#####"));

            Log.Debug($"Retrieving user data {authToken} from the web server {webServer}");

            // Retrieve user data from the web server.
            var client = new UserWebServiceClient(webServer);
            var member = client.GetMember(authToken);
            return member;
        }
    }
}
