using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Database;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public class ShopWebService : BaseShopWebService
    {
        private static class Events
        {
            public static readonly EventId BuyItem = new EventId(4000, nameof(BuyItem));
        }

        private readonly ILogger<ShopWebService> _logger;
        private readonly ItemManager _items;
        private readonly IDbService _database;
        private readonly ISessionService _sessions;

        // Cached responses.
        private readonly Task<UberStrikeItemShopClientView> _GetShopCache;

        public ShopWebService(
            ILogger<ShopWebService> logger,
            ItemManager items, 
            IDbService database,
            ISessionService sessions)
        {
            _logger = logger;
            _items = items;
            _database = database;
            _sessions = sessions;

            _GetShopCache = Task.FromResult(_items.GetView());
        }

        public override async Task<BuyItemResult> BuyItem(
            int itemId, 
            string authToken,
            UberStrikeCurrencyType currencyType,
            BuyingDurationType durationType,
            UberStrikeItemType itemType,
            BuyingLocationType marketLocation,
            BuyingRecommendationType recommendationType)
        {
            try
            {
                var item = _items.GetItem(itemId);
                if (item == null)
                    return BuyItemResult.ItemNotFound;

                var itemPrice = item.GetView().Prices
                                .Where(p => p.Currency == currencyType && p.Duration == durationType)
                                .FirstOrDefault();

                if (itemPrice == null)
                {
                    if (durationType == BuyingDurationType.Permanent)
                        return BuyItemResult.DisableForPermanent;
                    else
                        return BuyItemResult.DisableForRent;
                }

                var amount = itemPrice.IsConsumable ? itemPrice.Amount : -1;
                if (amount < -1)
                    return BuyItemResult.InvalidAmount;

                var member = await _sessions.GetMemberAsync(authToken);
                if (member == null || member.IsBanned())
                    return BuyItemResult.InvalidMember;

                if (member.Inventory.Items.ContainsKey(itemId))
                    return BuyItemResult.AlreadyInInventory;
                if (member.Level < item.GetView().LevelLock)
                    return BuyItemResult.InvalidLevel;

                var transactionDate = DateTime.UtcNow;
                var itemTransaction = new ItemTransaction
                {
                    ItemId = itemId,
                    Date = transactionDate,
                    Duration = durationType
                };

                if (itemPrice.Price > 0)
                {
                    switch (currencyType)
                    {
                        case UberStrikeCurrencyType.Points:
                            if (member.Points < itemPrice.Price)
                                return BuyItemResult.NotEnoughCurrency;
                            itemTransaction.Points = itemPrice.Price;
                            member.Points -= itemPrice.Price;
                            break;
                        case UberStrikeCurrencyType.Credits:
                            if (member.Credits < itemPrice.Price)
                                return BuyItemResult.NotEnoughCurrency;
                            itemTransaction.Credits = itemPrice.Price;
                            member.Credits -= itemPrice.Price;
                            break;

                        default:
                            return BuyItemResult.InvalidData;
                    }
                }

                var expiration = default(DateTime?);
                switch (durationType)
                {
                    case BuyingDurationType.Permanent:
                        expiration = null;
                        break;

                    case BuyingDurationType.NinetyDays:
                        expiration = transactionDate.AddDays(90);
                        break;
                    case BuyingDurationType.ThirtyDays:
                        expiration = transactionDate.AddDays(30);
                        break;
                    case BuyingDurationType.SevenDays:
                        expiration = transactionDate.AddDays(7);
                        break;
                    case BuyingDurationType.OneDay:
                        expiration = transactionDate.AddDays(1);
                        break;

                    default:
                        return BuyItemResult.InvalidData;
                }

                // Register to member's list of item transactions and add item
                // to member's inventory.
                // 
                // NOTE: Insert(0, transaction) so the list is ordered from
                // newest to oldest transaction.
                member.Transactions.Items.Insert(0, itemTransaction);
                member.Inventory.Items.Add(itemId, new InventoryItem
                {
                    ItemId = itemId,
                    AmountRemaining = amount,
                    Expiration = expiration
                });

                await _database.Members.UpdateAsync(member);
                _logger.LogDebug(Events.BuyItem, "\"{member}\" bought \"{item}\".", member, item);

                return BuyItemResult.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(Events.BuyItem, ex, "Failed to buy item \"{itemId}\".", itemId);
                return BuyItemResult.UnknownError;
            }
        }

        public override Task<UberStrikeItemShopClientView> GetShop()
            => _GetShopCache;
    }
}
