using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.Db
{
    public class UserDb
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(UserDb).Name);

        public UserDb()
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            Profiles = new ProfileDb();
            Wallets = new WalletDb();
            Inventories = new InventoryDb();
            Loadouts = new LoadoutDb();

            _ipBans = new HashSet<string>();
            _hwdBans = new HashSet<string>();

            if (!LoadSteamIds())
            {
                _steamId2Cmid = new Dictionary<string, int>();
                // Create the file if it does not exists.
                SaveSteamIds();
            }

            if (!LoadCmidBans())
            {
                _cmidBans = new HashSet<int>();
                // Create the file if it does not exists.
                SaveCmidBans();
            }

            if (!LoadUsedNames())
            {
                _usedNames = new HashSet<string>();
                // Create the file if it does not exists.
                SaveUsedNames();
            }
        }

        private Dictionary<string, int> _steamId2Cmid; // SteamID -> CMID

        private HashSet<int> _cmidBans;
        private HashSet<string> _ipBans;
        private HashSet<string> _hwdBans;
        private HashSet<string> _usedNames;

        public ProfileDb Profiles { get; }
        public WalletDb Wallets { get; }
        public InventoryDb Inventories { get; }
        public LoadoutDb Loadouts { get; }

        public bool Link(string steamId, MemberView member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (steamId == null)
                throw new ArgumentNullException(nameof(steamId));

            int cmid;
            if (_steamId2Cmid.TryGetValue(steamId, out cmid))
                return false; // Already linked.

            _steamId2Cmid.Add(steamId, member.PublicProfile.Cmid);

            // Save steam Ids after we done linking them.
            SaveSteamIds();
            return true;
        }

        public bool IsCmidBanned(int cmid)
        {
            return _cmidBans.Contains(cmid);
        }

        public bool IsHwdBanned(string hwd)
        {
            if (hwd == null)
                throw new ArgumentNullException(nameof(hwd));

            return _hwdBans.Contains(hwd);
        }

        public bool IsIpBanned(string ip)
        {
            if (ip == null)
                throw new ArgumentNullException(nameof(ip));

            return _ipBans.Contains(ip);
        }

        public void BanCmid(int cmid)
        {
            if (_cmidBans.Add(cmid))
                SaveCmidBans();
        }

        public bool UseName(string name)
        {
            lock (_usedNames)
            {
                if (_usedNames.Add(name))
                {
                    SaveUsedNames();
                    return true;
                }

                return false;
            }
        }

        public bool CanUseName(string name)
        {
            lock (_usedNames)
                return !_usedNames.Contains(name);
        }

        public MemberView LoadMember(string steamId)
        {
            if (steamId == null)
                throw new ArgumentNullException(nameof(steamId));

            // Try to find a linked CMID to the specified Steam ID.
            // If it does not, then return null, else load member with that
            // linked CMID.
            int cmid;
            if (!_steamId2Cmid.TryGetValue(steamId, out cmid))
                return null;

            return LoadMember(cmid);
        }

        public MemberView LoadMember(int cmid)
        {
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            var publicProfile = Profiles.Load(cmid);
            var memberWallet = Wallets.Load(cmid);
            var memberItems = new List<int>();

            var inventory = Inventories.Load(cmid);
            for (int i = 0; i < inventory.Count; i++)
                memberItems.Add(inventory[i].ItemId);

            return new MemberView(publicProfile, memberWallet, memberItems);
        }

        public int GetNextCmid()
        {
            if (!File.Exists("data/_nextcmid"))
                return -1;

            var file = File.ReadAllText("data/_nextcmid");
            return int.Parse(file);
        }

        public void SetNextCmid(int cmid)
        {
            if (cmid < 0)
                throw new ArgumentException();

            File.WriteAllText("data/_nextcmid", cmid.ToString());
        }

        private bool LoadCmidBans()
        {
            _cmidBans = Utils.DeserializeJsonAt<HashSet<int>>("data/bans.json");
            return _cmidBans != null;
        }

        private void SaveCmidBans()
        {
            if (_cmidBans == null)
                _cmidBans = new HashSet<int>();

            var json = JsonConvert.SerializeObject(_cmidBans);
            File.WriteAllText("data/bans.json", json);
        }

        private bool LoadUsedNames()
        {
            _usedNames = Utils.DeserializeJsonAt<HashSet<string>>("data/names.json");
            return _usedNames != null;
        }

        private void SaveUsedNames()
        {
            if (_usedNames == null)
                _usedNames = new HashSet<string>();

            var json = JsonConvert.SerializeObject(_usedNames);
            File.WriteAllText("data/names.json", json);
        }

        private bool LoadSteamIds()
        {
            _steamId2Cmid = Utils.DeserializeJsonAt<Dictionary<string, int>>("data/steam_id.json");
            return _steamId2Cmid != null;
        }

        private void SaveSteamIds()
        {
            if (_steamId2Cmid == null)
                _steamId2Cmid = new Dictionary<string, int>();

            var json = JsonConvert.SerializeObject(_steamId2Cmid);
            File.WriteAllText("data/steam_id.json", json);
        }
    }
}
