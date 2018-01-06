using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            _profilesDb = new ProfileDb();
            _walletsDb = new WalletDb();
            _inventoriesDb = new InventoryDb();
            _loadoutsDb = new LoadoutDb();

            if (!LoadSteamIds())
            {
                _steamId2Cmid = new Dictionary<string, int>();

                // Create the file if it does not exists.
                SaveSteamIds();
            }
        }

        private Dictionary<string, int> _steamId2Cmid; // SteamID -> CMID
        private readonly ProfileDb _profilesDb;
        private readonly WalletDb _walletsDb;
        private readonly InventoryDb _inventoriesDb;
        private readonly LoadoutDb _loadoutsDb;

        public ProfileDb Profiles => _profilesDb;
        public WalletDb Wallets => _walletsDb;
        public InventoryDb Inventories => _inventoriesDb;
        public LoadoutDb Loadouts => _loadoutsDb;

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

        private bool LoadSteamIds()
        {
            _steamId2Cmid = Utils.DeserializeJsonAt<Dictionary<string, int>>("data/steam_id.json");
            return _steamId2Cmid != null;
        }

        private void SaveSteamIds()
        {
            Debug.Assert(_steamId2Cmid != null);

            if (_steamId2Cmid == null)
                _steamId2Cmid = new Dictionary<string, int>();

            var json = JsonConvert.SerializeObject(_steamId2Cmid);
            File.WriteAllText("data/steam_id.json", json);
        }
    }
}
