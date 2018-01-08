namespace UberStrok.Realtime
{
    public enum IGamePeerOperationsType
    {
        SendHeartbeatResponse = 1,
        GetServerLoad,
        GetGameInformation,
        GetGameListUpdates,
        EnterRoom,
        CreateRoom,
        LeaveRoom,
        CloseRoom,
        InspectRoom,
        ReportPlayer,
        KickPlayer,
        UpdateLoadout,
        UpdatePing,
        UpdateKeyState,
        RefreshBackendData
    }
}
