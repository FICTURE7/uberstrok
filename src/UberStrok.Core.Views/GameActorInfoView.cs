using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    public class GameActorInfoView
    {
        public GameActorInfoView()
        {
            Weapons = new List<int>
            {
                0,
                0,
                0,
                0
            };
            Gear = new List<int>
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0
            };
            QuickItems = new List<int>
            {
                0,
                0,
                0
            };
            FunctionalItems = new List<int>
            {
                0,
                0,
                0
            };
        }

        public int Cmid { get; set; }
        public string PlayerName { get; set; }
        public MemberAccessLevel AccessLevel { get; set; }
        public ChannelType Channel { get; set; }
        public string ClanTag { get; set; }
        public byte Rank { get; set; }
        public byte PlayerId { get; set; }
        public PlayerStates PlayerState { get; set; }
        public short Health { get; set; }
        public TeamID TeamID { get; set; }
        public int Level { get; set; }
        public ushort Ping { get; set; }
        public byte CurrentWeaponSlot { get; set; }
        public FireMode CurrentFiringMode { get; set; }
        public byte ArmorPoints { get; set; }
        public byte ArmorPointCapacity { get; set; }
        public Color SkinColor { get; set; }
        public short Kills { get; set; }
        public short Deaths { get; set; }
        public List<int> Weapons { get; set; }
        public List<int> Gear { get; set; }
        public List<int> FunctionalItems { get; set; }
        public List<int> QuickItems { get; set; }
        public SurfaceType StepSound { get; set; }
    }
}
