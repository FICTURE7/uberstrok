using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class AccountCompletionResultView
	{
		public AccountCompletionResultView()
            : this(default, new Dictionary<int, int>(), new List<string>())
		{
            // Space
		}

		public AccountCompletionResultView(AccountCompletionResult result) 
            : this((int)result, new Dictionary<int, int>(), new List<string>())
		{
            // Space
		}

		public AccountCompletionResultView(int result, Dictionary<int, int> itemsAttributed, List<string> nonDuplicateNames)
		{
            Result = result;
            ItemsAttributed = itemsAttributed;
            NonDuplicateNames = nonDuplicateNames;
		}

		public Dictionary<int, int> ItemsAttributed { get; set; }
		public List<string> NonDuplicateNames { get; set; }
		public int Result { get; set; }
	}
}
