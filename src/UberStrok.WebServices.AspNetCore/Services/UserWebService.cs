using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Database;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public class UserWebService : BaseUserWebService
    {
        private static class Events
        {
            public static readonly EventId ChangeName = new EventId(3000, nameof(ChangeName));
        }

        private readonly IDbService _database;
        private readonly ISessionService _sessions;
        private readonly ILogger<UserWebService> _logger;

        public UserWebService(
            ILogger<UserWebService> logger,
            IDbService database,
            ISessionService sessions)
        {
            _logger = logger;
            _database = database;
            _sessions = sessions;
        }

        public override async Task<List<ItemInventoryView>> GetInventory(string authToken)
        {
            var member = await _sessions.GetMemberAsync(authToken);

            if (CheckMember(member))
                await _database.Members.UpdateAsync(member);

            var inventory = new List<ItemInventoryView>(member.Inventory.Items.Count);
            foreach (var item in member.Inventory.Items.Values)
            {
                inventory.Add(new ItemInventoryView
                {
                    Cmid = member.Id,
                    ItemId = item.ItemId,
                    ExpirationDate = item.Expiration,
                    AmountRemaining = item.AmountRemaining,
                });
            }

            return inventory;
        }

        public override async Task<LoadoutView> GetLoadout(string authToken)
        {
            // NOTE: The client validates a loadout by checking if it has it
            // has at least 1 item assigned in a slot. If the validation fails
            // then it will stop the authentication process.

            var member = await _sessions.GetMemberAsync(authToken);

            if (CheckMember(member))
                await _database.Members.UpdateAsync(member);

            return new LoadoutView
            {
                Cmid = member.Id,
                LoadoutId = member.Id
            }.From(member.Loadout);
        }

        public override async Task<MemberOperationResult> SetLoadout(string authToken, LoadoutView loadout)
        {
            var member = await _sessions.GetMemberOrNullAsync(authToken);
            if (member == null)
                return MemberOperationResult.MemberNotFound;

            CheckMember(member);

            // Load new loadout then strip expired items or no use remaining
            // items in CheckMemberLoadout.
            member.Loadout.Load(loadout);
            CheckMemberLoadout(member);

            await _database.Members.UpdateAsync(member);
            return MemberOperationResult.Ok;
        }

        public override async Task<UberstrikeUserView> GetMember(string authToken)
        {
            var member = await _sessions.GetMemberAsync(authToken);

            if (CheckMember(member))
                await _database.Members.UpdateAsync(member);

            return new UberstrikeUserView
            {
                CmuneMemberView = new MemberView().From(member),
                UberstrikeMemberView = new UberstrikeMemberView
                {
                    PlayerCardView = new PlayerCardView
                    {
                        Cmid = member.Id,
                        Name = member.Name,

                        // TODO: Implement.                        
                    },
                    PlayerStatisticsView = new PlayerStatisticsView().From(member)
                }
            };
        }

        public override async Task<MemberWalletView> GetMemberWallet(string authToken)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            return new MemberWalletView().From(member);
        }

        public override async Task<MemberOperationResult> ChangeMemberName(string authToken, string name, string locale, string machineId)
        {
            // TODO: Implement check for offensive names.

            const int NAME_CHANGE_ID = 1294;

            if (!NameHelpers.IsNameValid(name, locale))
                return MemberOperationResult.InvalidName;
            if (await _database.Members.NameExists(name))
                return MemberOperationResult.DuplicateName;

            var member = await _sessions.GetMemberOrNullAsync(authToken);
            if (member == null)
                return MemberOperationResult.MemberNotFound;
            if (!member.Inventory.Items.TryGetValue(NAME_CHANGE_ID, out InventoryItem inventoryItem))
                return MemberOperationResult.NameChangeNotInInventory;

            // Change name, remove use from name change & save to database.
            member.Name = name;
            inventoryItem.AmountRemaining--;

            CheckMember(member);

            await _database.Members.UpdateAsync(member);

            _logger.LogInformation(Events.ChangeName, "{cmid} changed name to \"{name}\"", member.Id, name);
            return MemberOperationResult.Ok;
        }

        public override Task<bool> IsDuplicateMemberName(string username)
            => _database.Members.NameExists(username);

        public override Task<List<string>> GenerateNonDuplicatedMemberNames(string username)
            // TODO: Implement.
            => Task.FromResult(new List<string>());

        public override async Task<ItemTransactionPageView> GetItemTransactions(string authToken, int pageIndex, int elementPerPage)
        {
            var member = await _sessions.GetMemberAsync(authToken);

            var baseIndex = (pageIndex - 1) * elementPerPage;
            var transactions = new List<ItemTransactionView>(elementPerPage);
            for (int i = 0; i < elementPerPage; i++)
            {
                var index = i + baseIndex;
                if (index >= member.Transactions.Items.Count)
                    break;
                transactions.Add(new ItemTransactionView().From(member.Transactions.Items[index]));
            }

            return new ItemTransactionPageView
            {
                ItemTransactions = transactions,
                TotalCount = member.Transactions.Items.Count
            };
        }

        public override async Task<PointsDepositPageView> GetPointsDeposits(string authToken, int pageIndex, int elementPerPage)
        {
            var member = await _sessions.GetMemberAsync(authToken);

            var baseIndex = (pageIndex - 1) * elementPerPage;
            var deposits = new List<PointsDepositView>(elementPerPage);
            for (int i = 0; i < elementPerPage; i++)
            {
                var index = i + baseIndex;
                if (index >= member.Transactions.Points.Count)
                    break;
                deposits.Add(new PointsDepositView().From(member.Transactions.Points[index]));
            }

            return new PointsDepositPageView
            {
                PointDeposits = deposits,
                TotalCount = member.Transactions.Points.Count
            };
        }

        // TODO: Consider extracting these to extension methods or maybe handle
        // the logic in LoadoutManager and InventoryManager?

        // Checks if the specified member's inventory and loadout.
        private static bool CheckMember(Member member)
            // NOTE: Order of checks is important, since CheckMemberLoadout
            // depends on the content of the member's inventory.
            => CheckMemberInventory(member) | CheckMemberLoadout(member);

        // Checks if the specified member's inventory for expired items.
        // Returns true if member at least an item from it was removed.
        private static bool CheckMemberInventory(Member member)
        {
            Debug.Assert(member != null);

            var now = DateTime.UtcNow;
            var expired = new List<int>();
            var inventory = member.Inventory.Items;

            // Find items which are expired or has no use left and register them
            // to 'expired'.
            foreach (var vk in inventory)
            {
                var itemId = vk.Key;
                var item = vk.Value;

                Debug.Assert(itemId == item.ItemId);

                if (now > item.Expiration || item.AmountRemaining == 0)
                    expired.Add(itemId);
            }

            foreach (var itemId in expired)
                inventory.Remove(itemId);

            return expired.Count > 0;
        }

        // Check if the specified member's loadout has items which are not in 
        // his inventory. Returns true if his loadout was modified.
        // 
        // NOTE: The client already does this check, but we do it on the server
        // as well, because security, amirite.
        private static bool CheckMemberLoadout(Member member)
        {
            Debug.Assert(member != null);
            
            var needUpdate = false;

            var inventory = member.Inventory.Items;
            var loadout = member.Loadout;

            // TODO: 
            // Not the most elegant code out there could probably use linq
            // expressions and stuff.

            if (loadout.GearFace != 0 && !inventory.ContainsKey(loadout.GearFace))
            {
                loadout.GearFace = 0;
                needUpdate = true;
            }
            if (loadout.GearFace != 0 && !inventory.ContainsKey(loadout.GearHead))
            {
                loadout.GearHead = 0;
                needUpdate = true;
            }
            if (loadout.GearGloves != 0 && !inventory.ContainsKey(loadout.GearGloves))
            {
                loadout.GearGloves = 0;
                needUpdate = true;
            }
            if (loadout.GearUpperBody != 0 && !inventory.ContainsKey(loadout.GearUpperBody))
            {
                loadout.GearUpperBody = 0;
                needUpdate = true;
            }
            if (loadout.GearLowerBody != 0 && !inventory.ContainsKey(loadout.GearLowerBody))
            {
                loadout.GearLowerBody = 0;
                needUpdate = true;
            }
            if (loadout.GearBoots != 0 && !inventory.ContainsKey(loadout.GearBoots))
            {
                loadout.GearBoots = 0;
                needUpdate = true;
            }
            if (loadout.WeaponMelee != 0 && !inventory.ContainsKey(loadout.WeaponMelee))
            {
                loadout.WeaponMelee = 0;
                needUpdate = true;
            }
            if (loadout.WeaponPrimary != 0 && !inventory.ContainsKey(loadout.WeaponPrimary))
            {
                loadout.WeaponPrimary = 0;
                needUpdate = true;
            }
            if (loadout.WeaponSecondary != 0 && !inventory.ContainsKey(loadout.WeaponSecondary))
            {
                loadout.WeaponSecondary = 0;
                needUpdate = true;
            }
            if (loadout.WeaponTertiary != 0 && !inventory.ContainsKey(loadout.WeaponTertiary))
            {
                loadout.WeaponTertiary = 0;
                needUpdate = true;
            }
            if (loadout.QuickItem1 != 0 && !inventory.ContainsKey(loadout.QuickItem1))
            {
                loadout.QuickItem1 = 0;
                needUpdate = true;
            }
            if (loadout.QuickItem2 != 0 && !inventory.ContainsKey(loadout.QuickItem2))
            {
                loadout.QuickItem2 = 0;
                needUpdate = true;
            }
            if (loadout.QuickItem3 != 0 && !inventory.ContainsKey(loadout.QuickItem3))
            {
                loadout.QuickItem3 = 0;
                needUpdate = true;
            }
            if (loadout.FunctionalItem1 != 0 && !inventory.ContainsKey(loadout.FunctionalItem1))
            {
                loadout.FunctionalItem1 = 0;
                needUpdate = true;
            }
            if (loadout.FunctionalItem2 != 0 && !inventory.ContainsKey(loadout.FunctionalItem2))
            {
                loadout.FunctionalItem2 = 0;
                needUpdate = true;
            }
            if (loadout.FunctionalItem3 != 0 && !inventory.ContainsKey(loadout.FunctionalItem3))
            {
                loadout.FunctionalItem3 = 0;
                needUpdate = true;
            }

            return needUpdate;
        }
    }
}
