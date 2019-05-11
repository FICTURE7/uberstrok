using log4net;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.Core
{
    public class AuthenticationWebService : BaseAuthenticationWebService
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(AuthenticationWebService).Name);

        public AuthenticationWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public override AccountCompletionResultView OnCompleteAccount(int cmid, string name, ChannelType channelType, string locale, string machineId)
        {
            if (Context.Configuration.Locked)
            {
                return new AccountCompletionResultView(
                    5,
                    null,
                    null
                );
            }

            if (!ValidName(name))
            {
                return new AccountCompletionResultView(
                    4,
                    null,
                    null
                );
            }

            var member = Context.Users.GetMember(cmid);
            if (member == null)
            {
                Log.Error("An unidentified CMID was passed.");
                return null;
            }

            var itemsAttributed = new Dictionary<int, int>();
            for (int i = 0; i < member.MemberItems.Count; i++) // The client seem to ignore the value of the dictionary.
                itemsAttributed.Add(member.MemberItems[i], 0);

            member.PublicProfile.Name = name;
            // Set email status to complete so we don't ask for the player name again.
            member.PublicProfile.EmailAddressStatus = EmailAddressStatus.Verified;

            if (Context.Users.Db.UseName(name))
                // Save the profile since we modified it.
                Context.Users.Db.Profiles.Save(member.PublicProfile);
            else
            {
                return new AccountCompletionResultView(
                    2,
                    null,
                    null
                );
            }

            /*
             * result:
             * 1 -> Success
             * 2 -> GetNonDuplicateNames?
             * 3 -> Load menu for show?
             * 4 -> Invalid characters in name
             * 5 -> Account banned
             */
            var view = new AccountCompletionResultView(
                1,
                itemsAttributed,
                null
            );
            return view;
        }

        public override MemberAuthenticationResultView OnLoginSteam(string steamId, string authToken, string machineId)
        {
            var ip = ((RemoteEndpointMessageProperty)OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name]).Address;

            if (Context.Users.Db.IsHwdBanned(machineId) || Context.Users.Db.IsIpBanned(ip))
            {
                return new MemberAuthenticationResultView
                {
                    MemberAuthenticationResult = MemberAuthenticationResult.IsBanned,
                    AuthToken = null,
                    IsAccountComplete = true,
                    ServerTime = DateTime.Now,

                    MemberView = null,
                    PlayerStatisticsView = null
                };
            }

            // Figure out if the account has been linked.
            var linked = true;
            // Figure out if the account existed. true -> existed otherwise false.
            var incomplete = false;

            // Load the user from the database using its steamId.
            var member = Context.Users.Db.LoadMember(steamId);
            if (member == null)
            {
                Log.Info($"Member entry {steamId} does not exists, creating new entry");

                // Create a new member if its not in the db.
                incomplete = true;
                member = Context.Users.NewMember();

                // Link the Steam ID to the CMID.
                linked = Context.Users.Db.Link(steamId, member);
            }
#if DEBUG
            else
            {
                var memoryMember = Context.Users.GetMember(member.PublicProfile.Cmid);
                if (memoryMember != null)
                {
                    member = Context.Users.NewMember();
                    Log.Info($"Faking member {memoryMember.PublicProfile.Cmid} with {member.PublicProfile.Cmid}");
                }
            }
#endif
            if (Context.Users.Db.IsCmidBanned(member.PublicProfile.Cmid))
            {
                return new MemberAuthenticationResultView
                {
                    MemberAuthenticationResult = MemberAuthenticationResult.IsBanned,
                    AuthToken = null,
                    IsAccountComplete = true,
                    ServerTime = DateTime.Now,

                    MemberView = null,
                    PlayerStatisticsView = null
                };
            }

            var result = MemberAuthenticationResult.Ok;
            if (!linked)
                result = MemberAuthenticationResult.InvalidEsns;

            // Use the PublicProfile.EmailAddrsessStatus to figure out if its an incomplete account,
            // because why not.
            if (member.PublicProfile.EmailAddressStatus == EmailAddressStatus.Unverified)
                incomplete = true;

            var session = Context.Users.LogInUser(member);
            session.Hwd = machineId;
            session.Ip = ip;

            var view = new MemberAuthenticationResultView
            {
                MemberAuthenticationResult = result,
                AuthToken = session.AuthToken,
                IsAccountComplete = !incomplete,
                ServerTime = DateTime.Now,

                MemberView = member,
                PlayerStatisticsView = new PlayerStatisticsView
                {
                    Cmid = member.PublicProfile.Cmid,
                    PersonalRecord = new PlayerPersonalRecordStatisticsView(),
                    WeaponStatistics = new PlayerWeaponStatisticsView()
                },
            };

            Log.Info($"Logging in member {steamId}:{session.AuthToken}");

            return view;
        }

        private static readonly Regex _nameRegex = new Regex("^[a-zA-Z0-9 .!_\\-<>{}~@#$%^&*()=+|:?]{3,18}$", RegexOptions.Compiled);

        private bool ValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return _nameRegex.IsMatch(name);
        }
    }
}
