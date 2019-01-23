using log4net;
using System;
using System.Diagnostics;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using System.Collections.Generic;

namespace UberStrok.Realtime.Server.Game
{
    public class CountdownMatchState : MatchState
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(CountdownMatchState));

        private double _countdown;
        private double _countdownOld;
        private DateTime _countdownEndTime;

        public CountdownMatchState(BaseGameRoom room) : base(room)
        {
            // Space
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;

            /* TODO: Allow user to set the countdown timer duration in a config or something. */
            _countdown = 5 * 1000;
            _countdownEndTime = DateTime.UtcNow.AddSeconds(_countdown);

            /* 
                Prepare all players by placing them in a 'prepare for next round state',
                and spawning them.
             */
            foreach (var player in Room.Players)
                PrepareAndSpawnPlayer(player);

            /* Start the game loop. */
            Room.StartLoop();
        }

        public override void OnResume()
        {
            // Space
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
        }

        public override void OnUpdate()
        {
            _countdownOld = _countdown;
            _countdown -= Room.Loop.DeltaTime.TotalMilliseconds;

            var countdownOldRound = (int)Math.Round(_countdownOld / 1000);
            var countdownRound = (int)Math.Round(_countdown / 1000);

            if (countdownOldRound < 0)
            {
                /* Make sure the countdown thingy is gone. */
                foreach (var player in Room.Players)
                    player.Events.Game.SendMatchStartCountdown(0);

                Room.State.Set(Id.Running);
            }
            else if (countdownOldRound > countdownRound)
            {
                foreach (var player in Room.Players)
                    player.Events.Game.SendMatchStartCountdown(countdownOldRound);
            }
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            /* Set players who just joined in the 'prepare for next round' state */
            PrepareAndSpawnPlayer(e.Player);
        }

        private void PrepareAndSpawnPlayer(GamePeer player)
        {
            var point = Room.SpawnManager.Get(player.Actor.Team);
            var movement = player.Actor.Movement;
            movement.Position = point.Position;
            movement.HorizontalRotation = point.Rotation;

            Debug.Assert(player.Actor.Info.PlayerId == player.Actor.Movement.Number);

            /*
                This prepares the client for the next round and enables match start
                countdown thingy.
             */
            player.State.Set(PeerState.Id.Countdown);

            // Reset stats, so if the player is rejoining they do not retain their previous match stats.
            player.WeaponStats = new Dictionary<int, WeaponStats>();
            player.CurrentLifeStats = new StatsCollectionView();
            player.StatsPerLife = new List<StatsCollectionView>();
            player.TotalStats = new StatsCollectionView();

            var actorView = new GameActorInfoView
            {
                // we don't want their team to change
                TeamID = player.Actor.Team,

                Health = 100,

                Deaths = 0,
                Kills = 0,

                Level = 1,

                Channel = ChannelType.Steam,
                PlayerState = PlayerStates.None,

                Ping = (ushort)(player.RoundTripTime / 2),

                Cmid = player.Member.CmuneMemberView.PublicProfile.Cmid,
                ClanTag = player.Member.CmuneMemberView.PublicProfile.GroupTag,
                AccessLevel = player.Member.CmuneMemberView.PublicProfile.AccessLevel,
                PlayerName = player.Member.CmuneMemberView.PublicProfile.Name,
            };

            /* Set the gears of the character. */
            /* Holo */
            actorView.Gear[0] = (int)player.Loadout.Type;
            actorView.Gear[1] = player.Loadout.Head;
            actorView.Gear[2] = player.Loadout.Face;
            actorView.Gear[3] = player.Loadout.Gloves;
            actorView.Gear[4] = player.Loadout.UpperBody;
            actorView.Gear[5] = player.Loadout.LowerBody;
            actorView.Gear[6] = player.Loadout.Boots;
            

            // Calculate armor capacity
            foreach (var armor in actorView.Gear)
            {
                var gear = default(UberStrikeItemGearView);
                if (Room.ShopManager.GearItems.TryGetValue(armor, out gear))
                    actorView.ArmorPointCapacity = (byte)Math.Min(200, actorView.ArmorPointCapacity + gear.ArmorPoints);
                else
                    s_log.Debug($"Could not find gear with ID {armor}.");
            }
            // Set armor on spawn to the max capacity
            actorView.ArmorPoints = actorView.ArmorPointCapacity;

            /* Sets the weapons of the character. */
            actorView.Weapons[0] = player.Loadout.MeleeWeapon;
            actorView.Weapons[1] = player.Loadout.Weapon1;
            actorView.Weapons[2] = player.Loadout.Weapon2;
            actorView.Weapons[3] = player.Loadout.Weapon3;

            var number = player.Actor.Number;
            var actor = new GameActor(actorView);
            player.Room = Room;
            player.Actor = actor;
            player.Actor.Number = number;

            /* Let all peers know that the player has joined the game. */
            foreach (var otherPeer in Room.Peers)
            {
                if (!otherPeer.KnownActors.Contains(player.Actor.Cmid))
                {
                    /*
                        PlayerJoinedGame event tells the client to initiate the character and register it
                        in its player list and update the team player number counts.
                     */
                    otherPeer.Events.Game.SendPlayerJoinedGame(player.Actor.Info.View, movement);
                    otherPeer.KnownActors.Add(player.Actor.Cmid);
                }

                otherPeer.Events.Game.SendPlayerRespawned(player.Actor.Cmid, movement.Position, movement.HorizontalRotation);
            }

            s_log.Debug($"Spawned: {player.Actor.Cmid} at: {point}");
        }
    }
}
