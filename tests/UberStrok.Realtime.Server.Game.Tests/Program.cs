using System;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.Realtime.Server.Game;

namespace UberStrok.Realtime.Server.Game.Tests
{
    public static class Program
    {
        public static void Main()
        {
            /*
            var pos1 = new Vector3(-6.082685f, 4.330345f, 50.79176f);
            var pos2 = new Vector3(27.18394f, 17.182f, -85.78404f);
            */

            /*
            var pos1 = new Vector3(-22.24f, 1.05f, 45.7f);
            var pos2 = new Vector3(-14.76f, 1.05f, 51.59f);
            */
             
            /*
            var pos1 = new Vector3(-4.06f, 1.05f, 47.53f);
            var pos2 = new Vector3(-7.31f, 1.05f, 49.76f);
            */

            var pos1 = new Vector3(-4.06f, 0f, 47.53f);
            var pos2 = new Vector3(-7.31f, 0f, 49.76f);

            var dir = pos1 - pos2;
            var forward = new Vector3(1, 0, 0);


            var angle = Vector3.Angle(forward, dir);
            if (dir.x < 0)
                angle = 360 - angle;

            var angleByte = Angle2Byte(angle);

            var angle2 = Byte2Angle(angleByte);
        }

        public static float Deg2Rad(float angle)
        {
            return Math.Abs((angle % 360f + 360f) % 360f / 360f);
        }

        public static float Byte2Angle(byte angle)
        {
            float num = 360f * (float)angle;
            return num / 255f;
        }

        public static byte Angle2Byte(float angle)
        {
            return (byte)(255f * Deg2Rad(angle));
        }

        public static void asdf(string[] args)
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
