namespace UberStrok.Realtime
{
    public enum ILobbyRoomEventsType
	{
		PlayerHide = 5,
		PlayerLeft,
		PlayerUpdate,
		UpdateContacts,
		FullPlayerListUpdate,
		PlayerJoined,
		ClanChatMessage,
		InGameChatMessage,
		LobbyChatMessage,
		PrivateChatMessage,
		UpdateInboxRequests,
		UpdateFriendsList,
		UpdateInboxMessages,
		UpdateClanMembers,
		UpdateClanData,
		UpdateActorsForModeration,
		ModerationCustomMessage,
		ModerationMutePlayer,
		ModerationKickGame
	}
}
