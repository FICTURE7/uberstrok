using System.Collections.Generic;

namespace UberStrok.Core.Views
{
    // Original 'ItemTransactionsViewModelProxy'
    public class ItemTransactionPageView
    {
        public List<ItemTransactionView> ItemTransactions { get; set; }
        public int TotalCount { get; set; }
    }
}
