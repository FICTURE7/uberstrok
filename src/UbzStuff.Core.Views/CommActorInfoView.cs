using System;
using UbzStuff.Core.Common;

namespace UbzStuff.Core.Views
{
    [Serializable]
    public class CommActorInfoView
    {
        public MemberAccessLevel AccessLevel { get; set; }
        public ChannelType Channel { get; set; }
        public string ClanTag { get; set; }
        public int Cmid { get; set; }
        public GameRoomView CurrentRoom { get; set; }
        public byte ModerationFlag { get; set; }
        public string ModInformation { get; set; }
        public string PlayerName { get; set; }
    }
}
