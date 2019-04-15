using log4net;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server
{
    public class BasePeer : ClientPeer
    {
        private readonly static ILog Log = LogManager.GetLogger(nameof(BasePeer));

        private static readonly Random _random = new Random();
        private static readonly SHA256 _hash = SHA256.Create();

        private enum ChallengeState
        {
            Success,
            Waiting,
            Failed
        }

        private string _expectedChallengeHash;
        private DateTime _challengeNextTime;
        private DateTime _challengeExpireTime;
        private ChallengeState _challengeState;

        private readonly byte[] _junkHash;
        private readonly byte[] _compositeHash;
        private readonly ConcurrentDictionary<int, BaseOperationHandler> _opHandlers;

        public string AuthToken { get; protected set; }
        public bool HasError { get; protected set; }

        public BasePeer(InitRequest request) : this(null, null, request)
        {
            /* Space */
        }

        public BasePeer(byte[] compositeHash, byte[] junkHash, InitRequest request) : base(request)
        {
            if (compositeHash != null && compositeHash.Length != 64)
                throw new ArgumentException(nameof(compositeHash));
            if (junkHash != null && junkHash.Length != 64)
                throw new ArgumentException(nameof(junkHash));

            _junkHash = junkHash;
            _compositeHash = compositeHash;

            if (_junkHash != null)
                _challengeNextTime = DateTime.UtcNow.AddSeconds(5);

            /* Check the client version. */
            if (request.ApplicationId != RealtimeVersion.Current)
                Disconnect();

            _opHandlers = new ConcurrentDictionary<int, BaseOperationHandler>();
        }

        /* TODO: Move those handler add/rm operations to another object (OperationHandlerCollection). */
        public void AddOperationHandler(BaseOperationHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (!_opHandlers.TryAdd(handler.Id, handler))
                throw new ArgumentException("Already contains a handler with the same handler ID.");
        }

        public void RemoveOperationHandler(int handlerId)
        {
            _opHandlers.TryRemove(handlerId, out BaseOperationHandler handler);
        }

        public void Update()
        {
            if (_challengeState == ChallengeState.Failed)
                DoError();
            if (_challengeState == ChallengeState.Waiting && DateTime.UtcNow >= _challengeExpireTime)
                Disconnect();

            if (_challengeState != ChallengeState.Waiting && _junkHash != null && DateTime.UtcNow >= _challengeNextTime)
                Challenge();
        }

        public bool Authenticate(string authToken, string magicHash)
        {
            if (authToken == null)
                throw new ArgumentNullException(nameof(authToken));
            if (magicHash == null)
                throw new ArgumentNullException(nameof(magicHash));

            Log.Info($"Authenticating {authToken}:{magicHash} at {RemoteIP}:{RemotePort}");
            var userView = GetMemberFromAuthToken(authToken);

            AuthToken = authToken;
            OnAuthenticate(userView);

#if !DEBUG
            if (_compositeHash != null &&
                userView.CmuneMemberView.PublicProfile.AccessLevel != MemberAccessLevel.Admin)
            {
                var authTokenBytes = Encoding.ASCII.GetBytes(authToken);
                var bytes = new byte[_compositeHash.Length + authTokenBytes.Length];
                Buffer.BlockCopy(_compositeHash, 0, bytes, 0, _compositeHash.Length);
                Buffer.BlockCopy(authTokenBytes, 0, bytes, _compositeHash.Length, authTokenBytes.Length);

                var magicHashBytes = _hash.ComputeHash(bytes);
                var reMagicHash = BytesToHexString(magicHashBytes);

                Log.Info($"MagicHash: {magicHash} == {reMagicHash}");
                if (reMagicHash != magicHash)
                    return false;
            }
#endif
            return true;
        }

        private void Challenge()
        {
            Debug.Assert(_junkHash != null);

            var hash = GenerateChallengeHash();

            var hashBytes = Encoding.ASCII.GetBytes(hash);
            var buffer = new byte[_junkHash.Length + hashBytes.Length];
            Buffer.BlockCopy(_junkHash, 0, buffer, 0, _junkHash.Length);
            Buffer.BlockCopy(hashBytes, 0, buffer, _junkHash.Length, hashBytes.Length);

            var responseBytes = _hash.ComputeHash(buffer);
            _expectedChallengeHash = BytesToHexString(responseBytes);

            _challengeExpireTime = DateTime.UtcNow.AddSeconds(5);
            _challengeState = ChallengeState.Waiting;

            Log.Info($"Challenge(): {hash} -> {_expectedChallengeHash}");
            DoHeartbeat(hash);
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
                _challengeNextTime = DateTime.UtcNow.AddSeconds(5);
                _challengeState = ChallengeState.Success;
                return true;
            }

            _expectedChallengeHash = null;
            _challengeState = ChallengeState.Failed;
            return false;
        }

        public virtual void DoHeartbeat(string hash)
        {
            /* Space */
        }

        public virtual void DoError(string message = "An error occured that forced UberStrike to halt.")
        {
            HasError = true;
        }

        protected virtual void OnAuthenticate(UberstrikeUserView userView)
        {
            /* Space */
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            foreach (var opHandler in _opHandlers.Values)
            {
                try { opHandler.OnDisconnect(this, reasonCode, reasonDetail); }
                catch (Exception ex)
                {
                    Log.Error($"Error while handling disconnection of peer -> {opHandler.GetType().Name}", ex);
                }
            }
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            /* 
                OperationRequest should contain 1 parameters.
                [0] -> (Int32 - OperationHandler ID) ->> (Byte[] - Data).

                Then we use OperationRequest.OperationCode & OperationHandler ID to,
                determine how to read stuff.

                Check if we've got enough parameters.
             */
            if (operationRequest.Parameters.Count < 1)
            {
                Log.Warn("Disconnecting client since its does not have enough parameters!");
                Disconnect();
                return;
            }

            var handlerId = operationRequest.Parameters.Keys.First();
            if (!_opHandlers.TryGetValue(handlerId, out BaseOperationHandler handler))
            {
                Log.Warn($"Unable to handle operation request -> not implemented operation handler: {handlerId}");
                return;
            }

            var data = (byte[])operationRequest.Parameters[handlerId];
            using (var bytes = new MemoryStream(data))
            {
                try { handler.OnOperationRequest(this, operationRequest.OperationCode, bytes); }
                catch (Exception ex)
                {
                    Log.Error($"Error while handling request {handler.GetType().Name}:{handlerId} -> OpCode: {operationRequest.OperationCode}", ex);
                }
            }
        }

        private static string BytesToHexString(byte[] bytes)
        {
            var builder = new StringBuilder(64);
            for (int i = 0; i < bytes.Length; i++)
                builder.Append(bytes[i].ToString("x2"));
            return builder.ToString();
        }

        private static string GenerateChallengeHash()
        {
            var buffer = new byte[32];
            _random.NextBytes(buffer);
            return BytesToHexString(buffer);
        }

        private static UberstrikeUserView GetMemberFromAuthToken(string authToken)
        {
            /* TODO: Provide some base class for this kind of server-server communications. */
            var bytes = Convert.FromBase64String(authToken);
            var data = Encoding.UTF8.GetString(bytes);
            var webServer = data.Substring(0, data.IndexOf("#####"));

            Log.Info($"Retrieving user data {authToken} from the web server {webServer}");
            /* Retrieve user data from the web server. */
            return new UserWebServiceClient(webServer).GetMember(authToken);
        }
    }
}
