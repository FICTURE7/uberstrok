using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game.Tests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var data = new GameActorInfoView
            {
                TeamID = TeamID.NONE,
                Health = 100,
                ArmorPoints = 0,
                ArmorPointCapacity = 0,
                Deaths = 0,
                Kills = 0,
                Level = 1,
                Channel = ChannelType.Steam,
                PlayerState = PlayerStates.Ready,
            };

            var actor = new GameActor(data);

            OnIsFiring(actor, true);
            OnIsFiring(actor, false);

            OnIsFiring(actor, false);
            OnIsPaused(actor, true);
        }

        public static void OnIsFiring(GameActor actor, bool on)
        {
            if (on)
                actor.Info.PlayerState |= PlayerStates.Shooting;
            else
                actor.Info.PlayerState &= ~PlayerStates.Shooting;
        }

        public static void OnIsPaused(GameActor actor, bool on)
        {
            if (on)
                actor.Info.PlayerState |= PlayerStates.Paused;
            else
                actor.Info.PlayerState &= ~PlayerStates.Paused;
        }
    }
}
