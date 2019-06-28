using log4net;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#if !DEBUG
using UberStrok.Core.Common;
#endif

using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server
{
    public abstract class Peer : ClientPeer
    {
        private static readonly Random _random = new Random();

        private enum HeartbeatState
        {
            Ok,
            Waiting,
            Failed
        }

        private string _heartbeat;
        private DateTime _heartbeatNextTime;
        private DateTime _heartbeatExpireTime;
        private HeartbeatState _heartbeatState;

        protected ILog Log { get; }

        protected UberstrikeUserView UserView { get; set; }

        public int HeartbeatTimeout { get; set; }
        public int HeartbeatInterval { get; set; }

        public bool HasError { get; protected set; }
        public string AuthToken { get; protected set; }
        public OperationHandlerCollection Handlers { get; }

        protected PeerConfiguration Configuration { get; }

        public Peer(InitRequest request) : base(request)
        {
            if (!(request.UserData is PeerConfiguration config))
                throw new ArgumentException("InitRequest.UserData was not a PeerConfiguration instance", nameof(request));

            /* Check the client version. */
            if (request.ApplicationId != RealtimeVersion.Current)
                throw new ArgumentException("InitRequest had an invalid application ID", nameof(request));

            Configuration = config;
            HeartbeatTimeout = config.HeartbeatTimeout;
            HeartbeatInterval = config.HeartbeatInterval;

            Log = LogManager.GetLogger(GetType().Name);
            Handlers = new OperationHandlerCollection();

            if (Configuration.JunkHashes.Count > 0)
                _heartbeatNextTime = DateTime.UtcNow.AddSeconds(HeartbeatInterval);
        }

        public virtual void Tick()
        {
            switch (_heartbeatState)
            {
                case HeartbeatState.Ok:
                    if (Configuration.JunkHashes.Count > 0 && DateTime.UtcNow >= _heartbeatNextTime)
                        Heartbeat();
                    break;
                case HeartbeatState.Waiting:
                    Debug.Assert(Configuration.JunkHashes.Count > 0);
                    if (DateTime.UtcNow >= _heartbeatExpireTime)
                        Disconnect();
                    break;
                case HeartbeatState.Failed:
                    Debug.Assert(Configuration.JunkHashes.Count > 0);
                    SendError();
                    break;
            }
        }

        public bool Authenticate(string authToken, string magicHash)
        {
            AuthToken = authToken ?? throw new ArgumentNullException(nameof(authToken));

            if (magicHash == null)
                throw new ArgumentNullException(nameof(magicHash));

            Log.Info($"Authenticating {authToken}:{magicHash} at {RemoteIP}:{RemotePort}");

            var userView = GetUser(true);
            OnAuthenticate(userView);

#if !DEBUG
            bool isAdmin = userView.CmuneMemberView.PublicProfile.AccessLevel != MemberAccessLevel.Admin;
            if (Configuration.CompositeHashes.Count > 0 && isAdmin)
            {
                var authTokenBytes = Encoding.ASCII.GetBytes(authToken);
                for (int i = 0; i < Configuration.CompositeHashes.Count; i++)
                {
                    var compositeHash = Configuration.CompositeHashes[i];
                    var reMagicHash = HashBytes(compositeHash, authTokenBytes);
                    if (reMagicHash == magicHash)
                    {
                        Log.Debug($"MagicHash: {reMagicHash} == {magicHash}");
                        return true;
                    }

                    Log.Error($"MagicHash: {reMagicHash} != {magicHash}");
                }

                return false;
            }
#endif
            return true;
        }

        public void Heartbeat()
        {
            if (Configuration.JunkHashes.Count == 0)
                return;

            _heartbeat = GenerateHeartbeat();
            _heartbeatExpireTime = DateTime.UtcNow.AddSeconds(HeartbeatTimeout);
            _heartbeatState = HeartbeatState.Waiting;

#if DEBUG_HEARTBEAT
            Log.Debug($"Heartbeat({_heartbeat}) with {HeartbeatTimeout}s timeout, expires at {_heartbeatExpireTime}");
#endif

            SendHeartbeat(_heartbeat);
        }

        public bool HeartbeatCheck(string responseHash)
        {
            if (responseHash == null)
                throw new ArgumentNullException(nameof(responseHash));

#if DEBUG_HEARTBEAT
            Log.Debug($"HeartbeatCheck({responseHash})");
#endif

            if (_heartbeat == null)
            {
                Log.Error("Heartbeat was null while checking.");
                return false;
            }

            for (int i = 0; i < Configuration.JunkHashes.Count; i++)
            {
                var junkBytes = Configuration.JunkHashes[i];
                var heartbeatBytes = Encoding.ASCII.GetBytes(_heartbeat);
                var expectedHeartbeat = HashBytes(junkBytes, heartbeatBytes);

                if (expectedHeartbeat == responseHash)
                {
#if DEBUG_HEARTBEAT
                    Log.Debug($"Heartbeat: {expectedHeartbeat} == {responseHash}");
#endif

                    _heartbeat = null;
                    _heartbeatNextTime = DateTime.UtcNow.AddSeconds(HeartbeatInterval);
                    _heartbeatState = HeartbeatState.Ok;
                    return true;
                }

                Log.Error($"Heartbeat: {expectedHeartbeat} != {responseHash}");
            }

            _heartbeat = null;
            _heartbeatState = HeartbeatState.Failed;
            return false;
        }

        public abstract void SendHeartbeat(string hash);

        public virtual void SendError(string message = "An error occured that forced UberStrike to halt.")
        {
            HasError = true;
        }

        public int Ban()
        {
            if (UserView == null)
                return 1;

            return new ModerationWebServiceClient(Configuration.WebServices)
                    .Ban(
                        Configuration.WebServicesAuth, 
                        UserView.CmuneMemberView.PublicProfile.Cmid
                     );
        }

        protected virtual void OnAuthenticate(UberstrikeUserView userView)
        {
            /* Space */
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            foreach (var handler in Handlers)
            {
                try
                {
                    handler.OnDisconnect(this, reasonCode, reasonDetail);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error while handling disconnection of peer on {handler.GetType().Name}", ex);
                }
            }
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            /* 
                OperationRequest should contain 1 value in its parameter dictionary.
                [0] -> (Int32: Channel.Id) ->> (Byte[]: RequestData).

                Check if we've got enough parameters.
             */
            if (operationRequest.Parameters.Count < 1)
            {
                Log.Warn($"Client at {RemoteIPAddress}:{RemotePort} did not send enough parameters. Disconnecting.");

                /* Assume protocol compliance failure, therefore disconnect. */
                Disconnect();
                return;
            }

            byte handlerId = operationRequest.Parameters.Keys.First();
            var handler = Handlers[handlerId];
            if (handler == null)
            {
                Log.Warn($"Client {RemoteIPAddress}:{RemotePort} sent an operation request on a handler which is not registered.");
                return;
            }

            if (!(operationRequest.Parameters[handlerId] is byte[] data))
            {
                Log.Warn($"Client {RemoteIPAddress} sent an operation request but the data type was not byte[]. Disconnecting.");

                /* Assume protocol compliance failure, therefore disconnect. */
                Disconnect();
                return;
            }

            using (var bytes = new MemoryStream(data))
            {
                try
                {
                    handler.OnOperationRequest(this, operationRequest.OperationCode, bytes);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error while handling request on {handler.GetType().Name} -> :{operationRequest.OperationCode}", ex);
                }
            }
        }

        public UberstrikeUserView GetUser(bool retrieve)
        {
            if (retrieve || UserView == null)
            {
                /* Retrieve user data from the web server. */
                Log.Debug($"Retrieving User from {Configuration.WebServices}");
                UserView = new UserWebServiceClient(Configuration.WebServices).GetMember(AuthToken);
            }

            return UserView;
        }

        private static string HashBytes(byte[] a, byte[] b)
        {
            var buffer = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, buffer, 0, a.Length);
            Buffer.BlockCopy(b, 0, buffer, a.Length, b.Length);

            byte[] hash = null;
            using (var sha256 = SHA256.Create())
                hash = sha256.ComputeHash(buffer);

            return BytesToHexString(hash);
        }

        private static string BytesToHexString(byte[] bytes)
        {
            var builder = new StringBuilder(64);
            for (int i = 0; i < bytes.Length; i++)
                builder.Append(bytes[i].ToString("x2"));
            return builder.ToString();
        }

        private static string GenerateHeartbeat()
        {
            var buffer = new byte[32];
            _random.NextBytes(buffer);
            return BytesToHexString(buffer);
        }
    }
}
