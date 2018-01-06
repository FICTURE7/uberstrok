using System;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommActor
    {
        public CommActor(CommPeer peer, CommActorInfoView view)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            _peer = peer;
            _view = view;
        }

        public int Cmid => _view.Cmid;
        public string Name => _view.PlayerName;

        public CommPeer Peer => _peer;
        public CommActorInfoView View => _view;

        private readonly CommPeer _peer;
        private readonly CommActorInfoView _view;
    }
}
