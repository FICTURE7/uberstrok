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
    public class Peer : ClientPeer
    {
        private static readonly Random _random = new Random();

        private enum HeartbeatState
        {
            Ok,
            Waiting,
            Failed
        }

        private readonly byte[] _junkHash;
        private readonly byte[] _compositeHash;

        private string _expectedHeartbeat;
        private DateTime _heartbeatNextTime;
        private DateTime _heartbeatExpireTime;
        private HeartbeatState _heartbeatState;

        protected ILog Log { get; }

        public int HeartbeatTimeout { get; set; }
        public int HeartbeatInterval { get; set; }

        public bool HasError { get; protected set; }
        public string AuthToken { get; protected set; }
        public OperationHandlerCollection Handlers { get; }

        public Peer(InitRequest request) : base(request)
        {
            if (!(request.UserData is PeerConfiguration config))
                throw new ArgumentException("InitRequest.UserData was not a PeerConfiguration instance", nameof(request));

            /* Check the client version. */
            if (request.ApplicationId != RealtimeVersion.Current)
                throw new ArgumentException("InitRequest had an invalid application ID", nameof(request));

            HeartbeatTimeout = config.HeartbeatTimeout;
            HeartbeatInterval = config.HeartbeatInterval;

            _junkHash = config.JunkBytes;
            _compositeHash = config.CompositeBytes;

            Log = LogManager.GetLogger(GetType().Name);
            Handlers = new OperationHandlerCollection();

            if (_junkHash != null)
                _heartbeatNextTime = DateTime.UtcNow.AddSeconds(HeartbeatInterval);
        }

        [Obsolete]
        public Peer(byte[] compositeHash, byte[] junkHash, InitRequest request) : base(request)
        {
            HeartbeatTimeout = 5;
            HeartbeatInterval = 5;

            if (compositeHash != null && compositeHash.Length != 64)
                throw new ArgumentException(nameof(compositeHash));
            if (junkHash != null && junkHash.Length != 64)
                throw new ArgumentException(nameof(junkHash));

            /* Check the client version. */
            if (request.ApplicationId != RealtimeVersion.Current)
            {
                Disconnect();
                return;
            }

            _junkHash = junkHash;
            _compositeHash = compositeHash;
            Log = LogManager.GetLogger(GetType().Name);
            Handlers = new OperationHandlerCollection();

            if (_junkHash != null)
                _heartbeatNextTime = DateTime.UtcNow.AddSeconds(HeartbeatInterval);
        }

        public void Update()
        {
            switch (_heartbeatState)
            {
                case HeartbeatState.Ok:
                    if (_junkHash != null && DateTime.UtcNow >= _heartbeatNextTime)
                        Heartbeat();
                    break;
                case HeartbeatState.Waiting:
                    Debug.Assert(_junkHash != null);
                    if (DateTime.UtcNow >= _heartbeatExpireTime)
                        Disconnect();
                    break;
                case HeartbeatState.Failed:
                    Debug.Assert(_junkHash != null);
                    SendError();
                    break;
            }
        }

        public bool Authenticate(string authToken, string magicHash)
        {
            if (authToken == null)
                throw new ArgumentNullException(nameof(authToken));
            if (magicHash == null)
                throw new ArgumentNullException(nameof(magicHash));

            Log.Info($"Authenticating {authToken}:{magicHash} at {RemoteIP}:{RemotePort}");
            var userView = GetMember(authToken);

            AuthToken = authToken;
            OnAuthenticate(userView);

#if !DEBUG
            bool isAdmin = userView.CmuneMemberView.PublicProfile.AccessLevel != MemberAccessLevel.Admin;
            if (_compositeHash != null && isAdmin)
            {
                var authTokenBytes = Encoding.ASCII.GetBytes(authToken);
                var reMagicHash = HashBytes(_compositeHash, authTokenBytes);
                if (reMagicHash != magicHash)
                {
                    Log.Error($"MagicHash: {magicHash} != {reMagicHash}");
                    return false;
                }

                Log.Info($"MagicHash: {magicHash} == {reMagicHash}");
            }
#endif
            return true;
        }

        private void Heartbeat()
        {
            Debug.Assert(_junkHash != null);

            var hash = GenerateHeartbeatHash();
            var hashBytes = Encoding.ASCII.GetBytes(hash);

            _expectedHeartbeat = HashBytes(_junkHash, hashBytes);

            _heartbeatExpireTime = DateTime.UtcNow.AddSeconds(HeartbeatTimeout);
            _heartbeatState = HeartbeatState.Waiting;

            Log.Info($"Heartbeat({hash}) -> {_expectedHeartbeat}");
            SendHeartbeat(hash);
        }

        public bool HeartbeatCheck(string responseHash)
        {
            if (responseHash == null)
                throw new ArgumentNullException(nameof(responseHash));

            if (_expectedHeartbeat == null)
            {
                Log.Error("Expected challenge hash was null while checking.");
                return false;
            }

            if (_expectedHeartbeat == responseHash)
            {
                Log.Info($"Heartbeat: {_expectedHeartbeat} == {responseHash}");
                _expectedHeartbeat = null;
                _heartbeatNextTime = DateTime.UtcNow.AddSeconds(HeartbeatInterval);
                _heartbeatState = HeartbeatState.Ok;
                return true;
            }

            Log.Error($"Heartbeat: {_expectedHeartbeat} != {responseHash}");
            _expectedHeartbeat = null;
            _heartbeatState = HeartbeatState.Failed;
            return false;
        }

        public virtual void SendHeartbeat(string hash)
        {
            /* Space */
        }

        public virtual void SendError(string message = "An error occured that forced UberStrike to halt.")
        {
            HasError = true;
        }

        protected virtual void OnAuthenticate(UberstrikeUserView userView)
        {
            /* Space */
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            foreach (var handler in Handlers)
            {
                try { handler.OnDisconnect(this, reasonCode, reasonDetail); }
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
                try { handler.OnOperationRequest(this, operationRequest.OperationCode, bytes); }
                catch (Exception ex)
                {
                    Log.Error($"Error while handling request on {handler.GetType().Name} -> :{operationRequest.OperationCode}", ex);
                }
            }
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

        private static string GenerateHeartbeatHash()
        {
            var buffer = new byte[32];
            _random.NextBytes(buffer);
            return BytesToHexString(buffer);
        }

        private static UberstrikeUserView GetMember(string authToken)
        {
            /* TODO: Provide some base class for this kind of server-server communications. */
            var bytes = Convert.FromBase64String(authToken);
            var data = Encoding.UTF8.GetString(bytes);
            var webServer = data.Substring(0, data.IndexOf("#####"));

            /* Retrieve user data from the web server. */
            return new UserWebServiceClient(webServer).GetMember(authToken);
        }
    }
}
