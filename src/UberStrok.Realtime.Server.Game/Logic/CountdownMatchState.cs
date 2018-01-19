using log4net;
using System;
using System.Diagnostics;

namespace UberStrok.Realtime.Server.Game.Logic
{
    public class CountdownMatchState : MatchState
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(CountdownMatchState));

        private double _countdown;
        private double _countdownOld;
        private DateTime _countdownEndTime;

        /* TODO: Calculate delta time in the game loop. */
        private double _deltaTime;

        private DateTime _lastUpdate;

        public CountdownMatchState(BaseGameRoom room) : base(room)
        {
            // Space
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;

            var now = DateTime.UtcNow;

            /* TODO: Allow user to set the countdown timer duration in a config or something. */
            _countdown = 5 * 1000;
            _countdownEndTime = now.AddSeconds(_countdown);
            _lastUpdate = now;

            /* 
                Prepare all players by placing them in a 'prepare for next round state',
                and spawning them.
             */
            foreach (var player in Room.Players)
                PrepareAndSpawnPlayer(player);

            Room.StartLoop();
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
        }

        public override void OnUpdate()
        {
            var now = DateTime.UtcNow;

            _countdownOld = _countdown;

            _deltaTime = (now - _lastUpdate).TotalMilliseconds;
            _countdown -= _deltaTime;

            var countdownOldRound = (int)Math.Round(_countdownOld / 1000);
            var countdownRound = (int)Math.Round(_countdown / 1000);

            if (countdownOldRound < 0)
                Room.State.Set(Id.Running);
            else if (countdownOldRound > countdownRound)
            {
                foreach (var player in Room.Players)
                    player.Events.Game.SendMatchStartCountdown(countdownOldRound);
            }

            _lastUpdate = DateTime.UtcNow;
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

            Debug.Assert(player.Actor.Data.PlayerId == player.Actor.Movement.Number);

            /*
                This prepares the client for the next round and enables match start
                countdown thingy.
             */
            player.Events.Game.SendPrepareNextRound();

            /* Let all peers know that the player has joined the game. */
            foreach (var otherPeer in Room.Peers)
            {
                if (!otherPeer.KnownActors.Contains(player.Actor.Cmid))
                    /*
                        PlayerJoinedGame event tells the client to initiate the character and register it
                        in its player list and update the team player number counts.
                     */
                    otherPeer.Events.Game.SendPlayerJoinedGame(player.Actor.Data, movement);

                otherPeer.Events.Game.SendPlayerRespawned(player.Actor.Cmid, movement.Position, movement.HorizontalRotation);
            }

            s_log.Debug($"Spawned: {player.Actor.Cmid} at: {point}");
        }
    }
}
