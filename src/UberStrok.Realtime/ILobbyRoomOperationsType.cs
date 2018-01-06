namespace UberStrok.Realtime
{
    public enum ILobbyRoomOperationsType
	{
		FullPlayerListUpdate = 1,
		UpdatePlayerRoom,
		ResetPlayerRoom,
		UpdateFriendsList,
		UpdateClanData,
		UpdateInboxMessages,
		UpdateInboxRequests,
		UpdateClanMembers,
		GetPlayersWithMatchingName,
		ChatMessageToAll,
		ChatMessageToPlayer,
		ChatMessageToClan,
		ModerationMutePlayer,
		ModerationPermanentBan,
		ModerationBanPlayer,
		ModerationKickGame,
		ModerationUnbanPlayer,
		ModerationCustomMessage,
		SpeedhackDetection,
		SpeedhackDetectionNew,
		PlayersReported,
		UpdateNaughtyList,
		ClearModeratorFlags,
		SetContactList,
		UpdateAllActors,
		UpdateContacts
	}
}
