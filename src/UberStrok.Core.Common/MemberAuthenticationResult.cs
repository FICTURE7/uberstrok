namespace UberStrok.Core.Common
{
	public enum MemberAuthenticationResult
	{
		Ok,
		InvalidData,
		InvalidName,
		InvalidEmail,
		InvalidPassword,
		IsBanned,
		InvalidHandle,
		InvalidEsns,
		InvalidCookie,
		IsIpBanned,
		UnknownError
	}
}
