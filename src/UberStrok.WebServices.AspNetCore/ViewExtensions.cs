using System;
using System.Linq;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public static class ViewExtensions
    {
        public static PublicProfileView From(this PublicProfileView view, Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return new PublicProfileView
            {
                Cmid = member.Id,
                Name = member.Name,
                GroupTag = member.Clan?.Tag,
                LastLoginDate = member.LastLogin,
                IsChatDisabled = member.IsMuted(),
                AccessLevel = member.AccessLevel,
                EmailAddressStatus = member.EmailStatus,

                FacebookId = default // Not used.
            };
        }

        public static MemberView From(this MemberView view, Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            view.PublicProfile = new PublicProfileView().From(member);
            view.MemberWallet = new MemberWalletView().From(member);
            view.MemberItems = member.Inventory.Items.Values.Select(p => p.ItemId).ToList();
            return view;
        }

        public static MemberWalletView From(this MemberWalletView view, Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            view.Cmid = member.Id;
            view.Credits = member.Credits;
            view.Points = member.Points;

            view.CreditsExpiration = default; // Not used.
            view.PointsExpiration = default; // Not used.
            return view;
        }

        public static LoadoutView From(this LoadoutView view, MemberLoadout loadout)
        {
            if (loadout == null)
                throw new ArgumentNullException(nameof(loadout));

            view.SkinColor = loadout.SkinColor;

            view.Webbing = loadout.GearHolo;

            view.Face = loadout.GearFace;
            view.Head = loadout.GearHead;
            view.Gloves = loadout.GearGloves;
            view.UpperBody = loadout.GearUpperBody;
            view.LowerBody = loadout.GearLowerBody;
            view.Boots = loadout.GearBoots;

            view.MeleeWeapon = loadout.WeaponMelee;
            view.Weapon1 = loadout.WeaponPrimary;
            view.Weapon2 = loadout.WeaponSecondary;
            view.Weapon3 = loadout.WeaponTertiary;

            view.QuickItem1 = loadout.QuickItem1;
            view.QuickItem2 = loadout.QuickItem2;
            view.QuickItem3 = loadout.QuickItem3;

            view.FunctionalItem1 = loadout.FunctionalItem1;
            view.FunctionalItem2 = loadout.FunctionalItem2;
            view.FunctionalItem3 = loadout.FunctionalItem3;

            // Not used.
            view.Weapon1Mod1 = default;
            view.Weapon1Mod2 = default;
            view.Weapon1Mod3 = default;

            // Not used.
            view.Weapon2Mod1 = default;
            view.Weapon2Mod2 = default;
            view.Weapon2Mod3 = default;

            // Not used.
            view.Weapon3Mod1 = default;
            view.Weapon3Mod2 = default;
            view.Weapon3Mod3 = default;
            // Not used.
            view.Type = default;
            view.Backpack = default;
            return view;
        }

        public static ItemTransactionView From(this ItemTransactionView view, ItemTransaction transaction)
        {
            view.WithdrawalDate = transaction.Date;
            view.ItemId = transaction.ItemId;
            view.Points = transaction.Points;
            view.Credits = transaction.Credits;
            view.Duration = transaction.Duration;

            view.Cmid = default; // Not used.
            view.WithdrawalId = default; // Not used.
            view.IsAdminAction = default; // Not used.
            return view;
        }

        public static PointsDepositView From(this PointsDepositView view, PointsDeposit deposit)
        {
            view.DepositType = deposit.Type;
            view.DepositDate = deposit.Date;
            view.Points = deposit.Points;

            view.Cmid = default; // Not used.
            view.PointDepositId = default; // Not used.
            view.IsAdminAction = default; // Not used.
            return view;
        }

        public static PrivateMessageView From(this PrivateMessageView view, PrivateMessage message)
        {
            view.PrivateMessageId = message.Id;
            view.IsRead = message.ReceiverRead;
            view.FromCmid = message.SenderMemberId;
            view.ToCmid = message.ReceiverMemberId;
            view.DateSent = message.Sent;
            view.ContentText = message.TextContent;

            view.HasAttachment = false; // Not used.
            view.IsDeletedByReceiver = false; // Not used.
            view.IsDeletedBySender = false; // Not used.
            return view;
        }

        public static PlayerStatisticsView From(this PlayerStatisticsView view, Member member)
        {
            view.Cmid = member.Id;
            view.Xp = member.Xp;
            view.Level = member.Level;
            view.Nutshots = member.Nutshots;
            view.Headshots = member.Headshots;
            view.Shots = member.Shots;
            view.Hits = member.Hits;
            view.Splats = member.Splats;
            view.Splatted = member.Splatted;

            // TODO: Implement.
            view.TimeSpentInGame = default;

            view.PersonalRecord = new PlayerPersonalRecordStatisticsView();
            view.WeaponStatistics = new PlayerWeaponStatisticsView();
            return view;
        }
    }
}
