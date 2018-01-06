using System;

namespace UberStrok.Core.Common
{
	[Serializable]
	public enum MemberOperationResult
	{
		Ok,
		DuplicateEmail = 2,
		DuplicateName,
		DuplicateHandle,
		DuplicateEmailName,
		MemberNotFound,
		InvalidData = 9,
		InvalidHandle,
		InvalidEsns,
		InvalidCmid,
		InvalidName,
		InvalidEmail,
		InvalidPassword,
		OffensiveName,
		NameChangeNotInInventory,
		AlreadyHasAnESNSAccountOfThisTypeAttached
	}
}
