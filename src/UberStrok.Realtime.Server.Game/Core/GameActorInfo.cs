using System;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game.Core
{
    public class GameActorInfo
    {
        private readonly GameActorInfoView _view;
        private readonly GameActorInfoDeltaView _viewDelta;

        public GameActorInfo(GameActorInfoView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            _view = view;
            _viewDelta = new GameActorInfoDeltaView();
        }

        /* Amount of tick the actor is in PlayerState.Shooting state. Used for SingleBulletFired.*/
        public int ShootingTick { get; set; }

        public int Cmid
        {
            get { return _view.Cmid; }
            set
            {
                if (value != Cmid)
                {
                    _view.Cmid = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Cmid] = value;
                }
            }
        }

        public string PlayerName
        {
            get { return _view.PlayerName; }
            set
            {
                if (value != PlayerName)
                {
                    _view.PlayerName = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.PlayerName] = value;
                }
            }
        }

        public MemberAccessLevel AccessLevel
        {
            get { return _view.AccessLevel; }
            set
            {
                if (value != AccessLevel)
                {
                    _view.AccessLevel = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.AccessLevel] = value;
                }
            }
        }

        public ChannelType Channel
        {
            get { return _view.Channel; }
            set
            {
                if (value != Channel)
                {
                    _view.Channel = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Channel] = value;
                }
            }
        }

        public string ClanTag
        {
            get { return _view.ClanTag; }
            set
            {
                if (value != ClanTag)
                {
                    _view.ClanTag = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.ClanTag] = value;
                }
            }
        }

        public byte Rank
        {
            get { return _view.Rank; }
            set
            {
                if (value != Rank)
                {
                    _view.Rank = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Rank] = value;
                }
            }
        }

        public byte PlayerId
        {
            get { return _view.PlayerId; }
            set
            {
                _viewDelta.Id = value;

                /* I don't think the client would like it, but whatever. */
                if (value != PlayerId)
                {
                    _view.PlayerId = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.PlayerId] = value;
                }
            }
        }

        public PlayerStates PlayerState
        {
            get { return _view.PlayerState; }
            set
            {
                if (value != PlayerState)
                {
                    _view.PlayerState = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.PlayerState] = value;
                }
            }
        }

        public short Health
        {
            get { return _view.Health; }
            set
            {
                if (value != Health)
                {
                    _view.Health = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Health] = value;
                }
            }
        }
        public TeamID TeamID
        {
            get { return _view.TeamID; }
            set
            {
                if (value != TeamID)
                {
                    _view.TeamID = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.TeamID] = value;
                }
            }
        }

        public int Level
        {
            get { return _view.Level; }
            set
            {
                if (value != Level)
                {
                    _view.Level = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Level] = value;
                }
            }
        }

        public ushort Ping
        {
            get { return _view.Ping; }
            set
            {
                if (value != Ping)
                {
                    _view.Ping = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Ping] = value;
                }
            }
        }

        public byte CurrentWeaponSlot
        {
            get { return _view.CurrentWeaponSlot; }
            set
            {
                if (value != CurrentWeaponSlot)
                {
                    _view.CurrentWeaponSlot = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.CurrentWeaponSlot] = value;
                }
            }
        }

        public FireMode CurrentFiringMode
        {
            get { return _view.CurrentFiringMode; }
            set
            {
                if (value != CurrentFiringMode)
                {
                    _view.CurrentFiringMode = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.CurrentFiringMode] = value;
                }
            }
        }

        public byte ArmorPoints
        {
            get { return _view.ArmorPoints; }
            set
            {
                if (value != ArmorPoints)
                {
                    _view.ArmorPoints = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.ArmorPoints] = value;
                }
            }
        }

        public byte ArmorPointCapacity
        {
            get { return _view.ArmorPointCapacity; }
            set
            {
                if (value != ArmorPointCapacity)
                {
                    _view.ArmorPointCapacity = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.ArmorPointCapacity] = value;
                }
            }
        }

        public Color SkinColor
        {
            get { return _view.SkinColor; }
            set
            {
                if (value != SkinColor)
                {
                    _view.SkinColor = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.SkinColor] = value;
                }
            }
        }

        public short Kills
        {
            get { return _view.Kills; }
            set
            {
                if (value != Kills)
                {
                    _view.Kills = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Kills] = value;
                }
            }
        }

        public short Deaths
        {
            get { return _view.Deaths; }
            set
            {
                if (value != Deaths)
                {
                    _view.Deaths = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Deaths] = value;
                }
            }
        }

        public List<int> Weapons
        {
            get { return _view.Weapons; }
            set
            {
                if (value != Weapons)
                {
                    _view.Weapons = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Weapons] = value;
                }
            }
        }

        public List<int> Gear
        {
            get { return _view.Gear; }
            set
            {
                if (value != Gear)
                {
                    _view.Gear = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Gear] = value;
                }
            }
        }

        public List<int> FunctionalItems
        {
            get { return _view.FunctionalItems; }
            set
            {
                if (value != FunctionalItems)
                {
                    _view.FunctionalItems = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.FunctionalItems] = value;
                }
            }
        }

        public List<int> QuickItems
        {
            get { return _view.QuickItems; }
            set
            {
                if (value != QuickItems)
                {
                    _view.QuickItems = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.QuickItems] = value;
                }
            }
        }

        public SurfaceType StepSound
        {
            get { return _view.StepSound; }
            set
            {
                if (value != StepSound)
                {
                    _view.StepSound = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.StepSound] = value;
                }
            }
        }

        public int CurrentWeaponID => _view.CurrentWeaponID;
        public GameActorInfoDeltaView ViewDelta => _viewDelta;
        public GameActorInfoView View => _view;
    }
}
