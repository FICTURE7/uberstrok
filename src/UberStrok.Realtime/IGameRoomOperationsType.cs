namespace UberStrok.Realtime
{
    public enum IGameRoomOperationsType
    {
        JoinGame = 1,
        JoinAsSpectator,
        PowerUpRespawnTimes,
        PowerUpPicked,
        IncreaseHealthAndArmor,
        OpenDoor,
        SpawnPositions,
        RespawnRequest,
        DirectHitDamage,
        ExplosionDamage,
        DirectDamage,
        DirectDeath,
        Jump,
        UpdatePositionAndRotation,
        KickPlayer,
        IsFiring,
        IsReadyForNextMatch,
        IsPaused,
        IsInSniperMode,
        SingleBulletFire,
        SwitchWeapon,
        SwitchTeam,
        ChangeGear,
        EmitProjectile,
        EmitQuickItem,
        RemoveProjectile,
        HitFeedback,
        ActivateQuickItem,
        ChatMessage
    }
}
