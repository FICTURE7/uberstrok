using Photon.SocketServer;
using System;
using System.Collections.Generic;
using UberStrok.Core;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeer : Peer
    {
        /* 
         * For when the peer changes its loadout and the game server is waiting 
         * for the web services to serve back.
         */
        public bool WaitingForLoadout { get; set; }

        public HashSet<int> KnownActors { get; set; }

        public ushort Ping { get; set; }
        public GameActor Actor { get; set; }

        public BaseGameRoom Room { get; set; }
        public LoadoutView Loadout { get; set; }
        public UberstrikeUserView Member { get; set; }

        public GamePeerEvents Events { get; }
        public StateMachine<PeerState.Id> State { get; }

        public GamePeer(InitRequest initRequest) : base(initRequest)
        {
            KnownActors = new HashSet<int>();
            Events = new GamePeerEvents(this);

            State = new StateMachine<PeerState.Id>();
            State.Register(PeerState.Id.None, null);
            State.Register(PeerState.Id.Overview, new OverviewPeerState(this));
            State.Register(PeerState.Id.WaitingForPlayers, new WaitingForPlayersPeerState(this));
            State.Register(PeerState.Id.Countdown, new CountdownPeerState(this));
            State.Register(PeerState.Id.Playing, new PlayingPeerState(this));
            State.Register(PeerState.Id.Killed, new KilledPeerState(this));

            Handlers.Add(new GamePeerOperationHandler());
        }

        public override void SendHeartbeat(string hash)
        {
            Events.SendHeartbeatChallenge(hash);
        }

        public override void SendError(string message = "An error occured that forced UberStrike to halt.")
        {
            base.SendError(message);
            Events.SendDisconnectAndDisablePhoton(message);
        }

        public void UpdateLoadout()
        {
            if (Room == null)
            {
                Log.Error("Tried to update loadout but was not in a room.");
                return;
            }

            WaitingForLoadout = true;

            /* Retrieve loadout from web services. */
            var loadout = GetLoadout();
            var weapons = new List<int>
            {
                loadout.MeleeWeapon,
                loadout.Weapon1,
                loadout.Weapon2,
                loadout.Weapon3
            };
            var gear = new List<int>
            {
                loadout.Webbing,
                loadout.Head,
                loadout.Face,
                loadout.Gloves,
                loadout.UpperBody,
                loadout.LowerBody,
                loadout.Boots
            };
            var quickItems = new List<int>
            {
                loadout.QuickItem1,
                loadout.QuickItem2,
                loadout.QuickItem3
            };

            var weaponViews = new List<UberStrikeItemWeaponView>();
            foreach (var itemId in weapons)
            {
                if (itemId == 0)
                    continue;

                weaponViews.Add(Room.Shop.WeaponItems[itemId]);
            }

            Actor.Weapons.Update(weaponViews);

            Actor.Info.Weapons = weapons;
            Actor.Info.Gear = gear;
            Actor.Info.QuickItems = quickItems;

            UpdateArmorCapacity();

            Loadout = loadout;
            WaitingForLoadout = false;
        }

        public void UpdateArmorCapacity()
        {
            if (Room == null)
            {
                Log.Error("Tried to update armor but was not in a room.");
                return;
            }

            /* Calculate armor capacity. */
            int armorCapacity = 0;
            foreach (var gearId in Actor.Info.Gear)
            {
                if (gearId == 0)
                    continue;

                if (!Room.Shop.GearItems.TryGetValue(gearId, out UberStrikeItemGearView gear))
                    throw new Exception($"Unable to find gear item with ID {gearId}");

                armorCapacity += gear.ArmorPoints;
            }

            /* Clamp armor capacity to 200. */
            Actor.Info.ArmorPointCapacity = Math.Min((byte)200, (byte)armorCapacity);
        }

        protected override void OnAuthenticate(UberstrikeUserView userView)
        {
            Member = userView;
            Loadout = GetLoadout();
        }
    }
}
