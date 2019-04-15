using log4net;
using Photon.SocketServer;
using System;
using System.Security.Cryptography;
using System.Text;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeer : BasePeer
    {
        private static readonly Random _random = new Random();
        private static readonly SHA256 _hash = SHA256.Create();

        private enum ChallengeState
        {
            None,
            Waiting,
            Failed,
            Success
        }

        private static readonly ILog Log = LogManager.GetLogger(nameof(CommPeer));

        public LobbyRoom Room { get; set; }
        public CommActor Actor { get; set; }
        public CommPeerEvents Events { get; }

        private string _expectedChallengeHash;
        private DateTime _challengeExpireTime;
        private ChallengeState _challengeState;

        public CommPeer(InitRequest request) : base(request)
        {
            Events = new CommPeerEvents(this);
            AddOperationHandler(new CommPeerOperationHandler());
        }

        public void Update()
        {
            if (_challengeState == ChallengeState.Failed)
                Events.SendDisconnectAndDisablePhoton();
            else if (_challengeState == ChallengeState.Waiting && DateTime.UtcNow >= _challengeExpireTime)
                Disconnect();
        }

        public bool Authenticate(string authToken, string magicHash)
        {
            if (authToken == null)
                throw new ArgumentNullException(nameof(authToken));
            if (magicHash == null)
                throw new ArgumentNullException(nameof(magicHash));

            Log.Info($"Authenticating {authToken}:{magicHash} at {RemoteIP}:{RemotePort}");

            var member = GetMemberFromAuthToken(authToken);
            var view = new CommActorInfoView
            {
                AccessLevel = member.CmuneMemberView.PublicProfile.AccessLevel,
                Channel = ChannelType.Steam,
                Cmid = member.CmuneMemberView.PublicProfile.Cmid,
                PlayerName = member.CmuneMemberView.PublicProfile.Name,
            };

            Actor = new CommActor(this, view);

#if !DEBUG
            if (CommApplication.Instance.Configuration.CompositeHash != null &&
                Actor.AccessLevel != MemberAccessLevel.Admin)
            {
                /* TODO: Cache this. */
                var compositeBytes = Encoding.ASCII.GetBytes(CommApplication.Instance.Configuration.CompositeHash);
                var authTokenBytes = Encoding.ASCII.GetBytes(authToken);
                var bytes = new byte[compositeBytes.Length + authTokenBytes.Length];
                Buffer.BlockCopy(compositeBytes, 0, bytes, 0, compositeBytes.Length);
                Buffer.BlockCopy(authTokenBytes, 0, bytes, compositeBytes.Length, authTokenBytes.Length);

                var magicHashBytes = _hash.ComputeHash(bytes);
                var reMagicHash = BytesToHexString(magicHashBytes);

                Log.Info($"MagicHash: {magicHash} -> {reMagicHash}");
                if (reMagicHash != magicHash)
                    return false;
            }
#endif
            return true;
        }

        public void Challenge()
        {
            var hash = GenerateChallengeHash();

            /* TODO: Cache this. */
            var junkBytes = Encoding.ASCII.GetBytes(CommApplication.Instance.Configuration.JunkHash);
            var hashBytes = Encoding.ASCII.GetBytes(hash);
            var buffer = new byte[junkBytes.Length + hashBytes.Length];
            Buffer.BlockCopy(junkBytes, 0, buffer, 0, junkBytes.Length);
            Buffer.BlockCopy(hashBytes, 0, buffer, junkBytes.Length, hashBytes.Length);

            var responseBytes = _hash.ComputeHash(buffer);
            _expectedChallengeHash = BytesToHexString(responseBytes);

            _challengeExpireTime = DateTime.UtcNow.AddSeconds(5);
            _challengeState = ChallengeState.Waiting;

            Log.Info($"Challenge hash: {hash} -> {_expectedChallengeHash}");

            Events.SendHeartbeatChallenge(hash);
        }

        public bool ChallengeCheck(string responseHash)
        {
            if (responseHash == null)
                throw new ArgumentNullException(nameof(responseHash));

            if (_expectedChallengeHash == null)
                return false; /* Should not happen. */

            if (_expectedChallengeHash == responseHash)
            {
                _expectedChallengeHash = null;
                _challengeState = ChallengeState.Success;
                return true;
            }

            _expectedChallengeHash = null;
            _challengeState = ChallengeState.Failed;
            return false;
        }

        private static string GenerateChallengeHash()
        {
            var buffer = new byte[16];
            _random.NextBytes(buffer);

            var hashBuffer = _hash.ComputeHash(buffer);
            return BytesToHexString(hashBuffer);
        }

        private static string BytesToHexString(byte[] bytes)
        {
            var builder = new StringBuilder(64);
            for (int i = 0; i < bytes.Length; i++)
                builder.Append(bytes[i].ToString("x2"));
            return builder.ToString();
        }

        private static UberstrikeUserView GetMemberFromAuthToken(string authToken)
        {
            /* TODO: Provide some base class for this kind of server-server communications. */
            var bytes = Convert.FromBase64String(authToken);
            var data = Encoding.UTF8.GetString(bytes);

            var webServer = data.Substring(0, data.IndexOf("#####"));

            Log.Debug($"Retrieving user data {authToken} from the web server {webServer}");

            /* Retrieve user data from the web server. */
            return new UserWebServiceClient(webServer).GetMember(authToken);
        }
    }
}
