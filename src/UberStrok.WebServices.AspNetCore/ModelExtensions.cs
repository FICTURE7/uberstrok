using System;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public static class ModelExtensions
    {
        public static bool IsBanned(this Member member)
        {
            if (member.BanExpiration is DateTime banExpiration)
            {
                if (DateTime.UtcNow <= banExpiration)
                    return true;
                // Lift ban if passed ban expiration.
                member.BanExpiration = null;
            }

            return false;
        }

        public static bool IsMuted(this Member member)
        {
            if (member.MuteExpiration is DateTime muteExpiration)
            {
                if (DateTime.UtcNow <= muteExpiration)
                    return true;
                // Lift mute if passed mute expiration.
                member.MuteExpiration = null;
            }

            return false;
        }

        public static void Load(this MemberLoadout loadout, LoadoutView view)
        {
            loadout.SkinColor = view.SkinColor;

            loadout.GearHolo = view.Webbing;

            loadout.GearFace = view.Face;
            loadout.GearHead = view.Head;
            loadout.GearGloves = view.Gloves;
            loadout.GearUpperBody = view.UpperBody;
            loadout.GearLowerBody = view.LowerBody;
            loadout.GearBoots = view.Boots;

            loadout.WeaponMelee = view.MeleeWeapon;
            loadout.WeaponPrimary = view.Weapon1;
            loadout.WeaponSecondary = view.Weapon2;
            loadout.WeaponTertiary = view.Weapon3;

            loadout.QuickItem1 = view.QuickItem1;
            loadout.QuickItem2 = view.QuickItem2;
            loadout.QuickItem3 = view.QuickItem3;

            loadout.FunctionalItem1 = view.FunctionalItem1;
            loadout.FunctionalItem2 = view.FunctionalItem2;
            loadout.FunctionalItem3 = view.FunctionalItem3;
        }
    }
}
