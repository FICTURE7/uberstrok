using System.Collections.Generic;

namespace UberStrok.Core.Views
{
    public class GameActorInfoDeltaView
    {
        public int DeltaMask { get; set; }
        public byte Id { get; set; }

        public readonly Dictionary<Keys, object> Changes = new Dictionary<Keys, object>();

        public void UpdateMask()
        {
            var mask = 0;
            foreach (var key in Changes.Keys)
                mask |= 1 << (int)key;

            DeltaMask = mask;
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
