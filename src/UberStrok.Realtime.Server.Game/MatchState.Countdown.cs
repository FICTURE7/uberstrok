using System.Diagnostics;
using UberStrok.Core;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public class CountdownMatchState : MatchState
    {
        private readonly Countdown _countdown;

        public CountdownMatchState(BaseGameRoom room) : base(room)
        {
            _countdown = new Countdown(Room.Loop, 5, 0);
            _countdown.Counted += OnCountdownCounted;
            _countdown.Completed += OnCountdownCompleted;
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;

            /* 
             * Prepare all players by placing them in a 'prepare for next round state',
             * and spawning them.
             */
            foreach (var player in Room.Players)
                PrepareAndSpawnPlayer(player);

            _countdown.Start();
        }

        public override void OnUpdate()
        {
            _countdown.Update();
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
        }

        private void OnCountdownCounted(int count)
        {
            foreach (var player in Room.Players)
                player.Events.Game.SendMatchStartCountdown(count);
        }

        private void OnCountdownCompleted()
        {
            Room.State.Set(Id.Running);
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            PrepareAndSpawnPlayer(e.Player);
        }

        /* Set the GamePeer in the `prepare for next round` state. */
        private void PrepareAndSpawnPlayer(GamePeer player)
        {
            player.Actor.Info.Health = 100;
            player.Actor.Info.Kills = 0;
            player.Actor.Info.Deaths = 0;
            player.Actor.Info.PlayerState &= ~PlayerStates.Dead;

            var point = Room.Spawns.Get(player.Actor.Team);
            var movement = player.Actor.Movement;
            movement.Position = point.Position;
            movement.HorizontalRotation = point.Rotation;

            Debug.Assert(player.Actor.Info.PlayerId == player.Actor.Movement.Number);

            /*
             * This prepares the client for the next round and enables match start
             * countdown thingy.
             */
            player.State.Set(PeerState.Id.Countdown);

            /* Let all peers know that the player has joined the game. */
            foreach (var otherPeer in Room.Peers)
            {
                if (!otherPeer.KnownActors.Contains(player.Actor.Cmid))
                {
                    /*
                     * PlayerJoinedGame event tells the client to initiate the character and register it
                     * in its player list and update the team player number counts.
                     */
                    otherPeer.Events.Game.SendPlayerJoinedGame(player.Actor.Info.View, movement);
                    otherPeer.KnownActors.Add(player.Actor.Cmid);
                }

                otherPeer.Events.Game.SendPlayerRespawned(player.Actor.Cmid, movement.Position, movement.HorizontalRotation);
            }

            Log.Debug($"Spawned: {player.Actor.Cmid} at: {point}");
        }
    }
}
