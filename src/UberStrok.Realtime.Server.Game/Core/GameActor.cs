using System;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameActor
    {
        public GameActor(GameActorInfoView data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Info = new GameActorInfo(data);
            Movement = new PlayerMovement();
            Damages = new DamageEventView();
            Weapons = new WeaponManager();
            Projectiles = new ProjectileManager();
            Ping = new PingManager();
        }

        public TeamID Team
        {
            get { return Info.TeamID; }
            set { Info.TeamID = value; }
        }

        public int Number
        {
            get { return Info.PlayerId; }
            set
            {
                Info.PlayerId = (byte)value;
                Movement.Number = (byte)value;
            }
        }

        public int Cmid => Info.Cmid;
        public string PlayerName => Info.PlayerName;
        public MemberAccessLevel AccessLevel => Info.AccessLevel;

        public PingManager Ping { get; }
        public ProjectileManager Projectiles { get; }
        public WeaponManager Weapons { get; }
        public DamageEventView Damages { get; }
        public PlayerMovement Movement { get; }
        public GameActorInfo Info { get; }
    }
}
