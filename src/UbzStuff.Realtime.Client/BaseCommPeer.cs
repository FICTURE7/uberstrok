namespace UbzStuff.Realtime.Client
{
    public abstract class BaseCommPeer : BasePeer, IEventDispatcher
    {
        public BaseCommPeer()
        {
            _operations = new CommPeerOperations(this, 1);

            AddDispatcher(this);
        }

        public CommPeerOperations Operations => _operations;
        private readonly CommPeerOperations _operations;

        protected abstract void OnLobbyEntered();

        void IEventDispatcher.OnEvent(byte opCode, byte[] data)
        {
            var op = (ICommPeerEventsType)opCode;
            switch (op)
            {
                case ICommPeerEventsType.HeartbeatChallenge:
                    break;

                case ICommPeerEventsType.DisconnectAndDisablePhoton:
                    break;

                case ICommPeerEventsType.LoadData:
                    break;

                case ICommPeerEventsType.LobbyEntered:
                    OnLobbyEntered();
                    break;
            }
        }
    }
}
