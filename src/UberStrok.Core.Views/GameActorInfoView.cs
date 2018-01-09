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

        public bool IsFiring => this.Is(PlayerStates.Shooting);
        public bool IsReadyForGame => this.Is(PlayerStates.Ready);
        public bool IsOnline => !this.Is(PlayerStates.Offline);
        public int CurrentWeaponID => (this.Weapons == null || this.Weapons.Count <= (int)this.CurrentWeaponSlot) ? 0 : this.Weapons[(int)this.CurrentWeaponSlot];
        public bool Is(PlayerStates state) => (byte)(this.PlayerState & state) != 0;
        public bool IsAlive => (byte)(this.PlayerState & PlayerStates.Dead) == 0;
        public bool IsSpectator => (byte)(this.PlayerState & PlayerStates.Spectator) != 0;
        public float GetAbsorptionRate() => 0.66f;
    }
}
