using System.Collections.Generic;

namespace UberStrok.Core.Views
{
    public class GameActorInfoDeltaView
    {
        /* Original identifier `Id`. */
        public byte PlayerId { get; set; }
        public int DeltaMask { get; set; }

        public Dictionary<Keys, object> Changes { get; }

        public GameActorInfoDeltaView()
        {
            Changes = new Dictionary<Keys, object>();
        }

        public void Update()
        {
            int mask = 0;
            foreach (var key in Changes.Keys)
                mask |= 1 << (int)key;

            DeltaMask = mask;
        }

        public void Reset()
        {
            Changes.Clear();
            Update();
        }

        public enum Keys
        {
            AccessLevel,
            ArmorPointCapacity,
            ArmorPoints,
            Channel,
            ClanTag,
            Cmid,
            CurrentFiringMode,
            CurrentWeaponSlot,
            Deaths,
            FunctionalItems,
            Gear,
            Health,
            Kills,
            Level,
            Ping,
            PlayerId,
            PlayerName,
            PlayerState,
            QuickItems,
            Rank,
            SkinColor,
            StepSound,
            TeamID,
            Weapons
        }
    }
}
