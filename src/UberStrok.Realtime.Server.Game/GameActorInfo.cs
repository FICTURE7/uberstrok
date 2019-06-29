using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameActorInfo
    {
        private readonly GameActorInfoView _view;
        private readonly GameActorInfoDeltaView _viewDelta;

        public GameActorInfo()
        {
            _view = new GameActorInfoView();
            _viewDelta = new GameActorInfoDeltaView();
        }

        public bool IsAlive => !Is(PlayerStates.Dead);
        public bool IsOnline => !Is(PlayerStates.Offline);
        public bool IsSpectator => Is(PlayerStates.Spectator);
        public bool IsShooting => Is(PlayerStates.Shooting);
        public bool IsReady => Is(PlayerStates.Ready);
        public int CurrentWeaponID => (Weapons == null || Weapons.Count <= CurrentWeaponSlot) ? 0 : Weapons[CurrentWeaponSlot];

        public int Cmid
        {
            get => _view.Cmid;
            set
            {
                if (value != Cmid)
                {
                    _view.Cmid = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Cmid] = value;
                }
            }
        }

        public byte PlayerId
        {
            get => _view.PlayerId;
            set
            {
                /* 
                 * I don't think the client would like it, but whatever; should
                 * also change the PlayerId associated with the 
                 * `PlayerMovement` of the actor.
                 */
                if (value != PlayerId)
                {
                    _view.PlayerId = value;
                    _viewDelta.PlayerId = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.PlayerId] = value;
                }
            }
        }

        public string PlayerName
        {
            get => _view.PlayerName;
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
            get => _view.AccessLevel;
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
            get => _view.Channel;
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
            get => _view.ClanTag;
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
            get => _view.Rank;
            set
            {
                if (value != Rank)
                {
                    _view.Rank = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.Rank] = value;
                }
            }
        }

        public PlayerStates PlayerState
        {
            get => _view.PlayerState;
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
            get => _view.Health;
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
            get => _view.TeamID;
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
            get => _view.Level;
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
            get => _view.Ping;
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
            get => _view.CurrentWeaponSlot;
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
            get => _view.CurrentFiringMode;
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
            get => _view.ArmorPoints;
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
            get => _view.ArmorPointCapacity;
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
            get => _view.SkinColor;
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
            get => _view.Kills;
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
            get => _view.Deaths;
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
            get => _view.Weapons;
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
            get => _view.Gear;
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
            get => _view.FunctionalItems;
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
            get => _view.QuickItems;
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
            get => _view.StepSound;
            set
            {
                if (value != StepSound)
                {
                    _view.StepSound = value;
                    _viewDelta.Changes[GameActorInfoDeltaView.Keys.StepSound] = value;
                }
            }
        }

        public bool Is(PlayerStates state)
        {
            return (byte)(PlayerState & state) == (byte)state;
        }

        public float GetAbsorptionRate()
        {
            return 0.66f;
        }

        public GameActorInfoView GetView()
        {
            return _view;
        }

        public GameActorInfoDeltaView GetViewDelta()
        {
            return _viewDelta;
        }
    }
}
