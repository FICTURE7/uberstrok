namespace UberStrok.Core.Common
{
    public enum BuyItemResult
	{
		OK,
		DisableInShop,
		DisableForRent = 3,
		DisableForPermanent,
		DurationDisabled,
		PackDisabled,
		IsNotForSale,
		NotEnoughCurrency,
		InvalidMember,
		InvalidExpirationDate,
		AlreadyInInventory,
		InvalidAmount,
		NoStockRemaining,
		InvalidData,
		TooManyUsage,
		InvalidLevel = 100,
		ItemNotFound = 404,

        UnknownError = -1, // UberStrok extension.
	}
}
