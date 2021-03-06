﻿using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.Db;

namespace UberStrok.WebServices
{
    public class UserManager
    {
        public class Session
        {
            public string AuthToken { get; set; }
            public string Ip { get; set; }
            public string Hwd { get; set; }
            public MemberView Member { get; set; }
        }

        private readonly static ILog Log = LogManager.GetLogger(typeof(UserManager).Name);

        private int _nextCmid;
        private readonly Dictionary<string, Session> _sessions; // AuthToken -> Session

        private readonly WebServiceContext _ctx;

        public UserManager(WebServiceContext ctx)
        {
            _ctx = ctx;

            Db = new UserDb();
            Authed = new HashSet<string>();

            _sessions = new Dictionary<string, Session>();
            _nextCmid = Db.GetNextCmid();
            if (_nextCmid == -1)
            {
                _nextCmid = 0;
                Db.SetNextCmid(_nextCmid);
            }
        }


        public HashSet<string> Authed { get; }
        public UserDb Db { get; }

        public MemberView NewMember()
        {
            var cmid = Interlocked.Increment(ref _nextCmid);
            var publicProfile = new PublicProfileView(
                cmid,
                "Player",
                MemberAccessLevel.Default,
                false,
                DateTime.UtcNow,
                EmailAddressStatus.Unverified,
                "-1"
            );

            var memberWallet = new MemberWalletView(
                cmid,
                _ctx.Configuration.Wallet.StartingCredits,
                _ctx.Configuration.Wallet.StartingPoints,
                DateTime.MaxValue,
                DateTime.MaxValue
            );

            var memberInventories = new List<ItemInventoryView>
            {
                new ItemInventoryView(1, null, -1, cmid),
                new ItemInventoryView(12, null, -1, cmid)
            };

            //TODO: Create helper function for conversion of this stuff.
            var memberItems = new List<int>();
            for (int i = 0; i < memberInventories.Count; i++)
                memberItems.Add(memberInventories[i].ItemId);

            var memberLoadout = new LoadoutView
            {
                Cmid = cmid,
                MeleeWeapon = 1,
                Weapon1 = 12
            };

            var member = new MemberView(
                publicProfile,
                memberWallet,
                memberItems
            );

            // Save the member.
            Db.Profiles.Save(publicProfile);
            Db.Wallets.Save(memberWallet);
            Db.Inventories.Save(cmid, memberInventories);
            Db.Loadouts.Save(memberLoadout);

            Db.SetNextCmid(_nextCmid);

            return member;
        }

        public MemberView GetMember(string authToken)
        {
            if (authToken == null)
                throw new ArgumentNullException(nameof(authToken));

            var session = default(Session);
            lock (_sessions)
            {
                if (!_sessions.TryGetValue(authToken, out session))
                    return null;
            }
            return session.Member;
        }

        public MemberView GetMember(int cmid)
        {
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            lock (_sessions)
            {
                foreach (var value in _sessions.Values)
                {
                    if (value.Member.PublicProfile.Cmid == cmid)
                        return value.Member;
                }
            }
            return null;
        }

        public Session GetSession(int cmid)
        {
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            lock (_sessions)
            {
                foreach (var value in _sessions.Values)
                {
                    if (value.Member.PublicProfile.Cmid == cmid)
                        return value;
                }
            }
            return null;
        }

        public Session LogInUser(MemberView member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            // Encode ServiceBase URL into the AuthToken so the realtime servers can figure out
            // where the user came from.
            var data = _ctx.ServiceBase + "#####" + DateTime.UtcNow.ToFileTime();
            var bytes = Encoding.UTF8.GetBytes(data);
            var authToken = Convert.ToBase64String(bytes);
            var session = default(Session);

            member.PublicProfile.LastLoginDate = DateTime.UtcNow;

            lock (_sessions)
            {
                foreach (var kv in _sessions)
                {
                    var value = kv.Value;
                    if (value.Member.PublicProfile.Cmid == member.PublicProfile.Cmid)
                    {
                        /* Replace players with same CMID, not the neatest of fixes, but it works. */
                        _sessions.Remove(kv.Key);
                        Log.Info($"Kicking player with CMID {value.Member.PublicProfile.Cmid} cause of new login.");
                        break;
                    }
                }

                session = new Session
                {
                    AuthToken = authToken,
                    Member = member,
                    Ip = null,
                    Hwd = null
                };

                _sessions.Add(authToken, session);
            }

            // Save only profile since we only modified the profile.
            Db.Profiles.Save(member.PublicProfile);
            return session;
        }

        public bool LogOutUser(MemberView member)
        {
            foreach (var kv in _sessions)
            {
                var value = kv.Value;
                if (value.Member.PublicProfile.Cmid == member.PublicProfile.Cmid)
                {
                    /* Replace players with same CMID, not the neatest of fixes, but it works. */
                    _sessions.Remove(kv.Key);
                    Log.Info($"Player with CMID {value.Member.PublicProfile.Cmid} logged out.");
                     return true;
                }
            }

            return false;
        }
    }
}
