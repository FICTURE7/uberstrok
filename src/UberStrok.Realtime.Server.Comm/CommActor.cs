using System;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommActor
    {
        public CommPeer Peer { get; }
        public CommActorInfoView View { get; }

        public int Cmid => View.Cmid;
        public string Name => View.PlayerName;
        public MemberAccessLevel AccessLevel => View.AccessLevel;

        public CommActor(CommPeer peer, CommActorInfoView view)
        {
            Peer = peer ?? throw new ArgumentNullException(nameof(peer));
            View = view ?? throw new ArgumentNullException(nameof(view));
        }
    }
}
