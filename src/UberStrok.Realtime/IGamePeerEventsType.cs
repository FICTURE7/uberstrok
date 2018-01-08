namespace UberStrok.Realtime
{
    public enum IGamePeerEventsType
    {
        HeartbeatChallenge = 1,
        RoomEntered,
        RoomEnterFailed,
        RequestPasswordForRoom,
        RoomLeft,
        FullGameList,
        GameListUpdate,
        GameListUpdateEnd,
        GetGameInformation,
        ServerLoadData,
        DisconnectAndDisablePhoton
    }
}
