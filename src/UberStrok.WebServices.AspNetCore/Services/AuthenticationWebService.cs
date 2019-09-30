using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Configurations;
using UberStrok.WebServices.AspNetCore.Database;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public class AuthenticationWebService : BaseAuthenticationWebService
    {
        private static class Events
        {
            public static readonly EventId Login = new EventId(2000, nameof(Login));
            public static readonly EventId CreateAccount = new EventId(2001, nameof(CreateAccount));
            public static readonly EventId CompleteAccount = new EventId(2002, nameof(CompleteAccount));
        }

        private readonly HashSet<string> _auths;

        private readonly ItemManager _items;
        private readonly AccountConfiguration _accountConfig;
        private readonly IDbService _database;
        private readonly ISessionService _sessions;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<AuthenticationWebService> _logger;

        public AuthenticationWebService(
            ILogger<AuthenticationWebService> logger,
            IHostingEnvironment env,
            IOptions<AccountConfiguration> accConfig,
            IDbService database,
            ISessionService sessions,
            ItemManager items)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sessions = sessions ?? throw new ArgumentNullException(nameof(sessions));
            _accountConfig = accConfig.Value;

            if (_env.IsDevelopment())
                _auths = new HashSet<string>();
        }

        public override async Task<AccountCompletionResultView> CompleteAccount(int cmid, string name, ChannelType channelType, string locale, string machineId)
        {
            try
            {
                var member = await _database.Members.FindAsync(cmid);
                if (member == null)
                {
                    _logger.LogError(Events.CompleteAccount, "No member associated with CMID \"{cmid}\".", cmid);
                    // Let client know something is wrong.
                    return null;
                }
                else if (member.EmailStatus == EmailAddressStatus.Verified)
                {
                    _logger.LogError(Events.CompleteAccount, "Member associated with CMID \"{cmid}\" already completed.", cmid);
                    // Let client know something is wrong.
                    return null;
                }

                if (!NameHelpers.IsNameValid(name, locale))
                    return new AccountCompletionResultView(AccountCompletionResult.InvalidName);
                if (await _database.Members.NameExists(name))
                    return new AccountCompletionResultView(AccountCompletionResult.DuplicateName);

                // Complete member with the desired name and get the items
                // which was attribtued to the new member.
                var itemsAttributed = await CompleteMember(member, name);

                return new AccountCompletionResultView
                {
                    // TODO: Change type of Result from int to AccountCompleteResult.
                    Result = (int)AccountCompletionResult.Ok,
                    ItemsAttributed = itemsAttributed,
                    NonDuplicateNames = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(Events.CompleteAccount, ex, "Failed to complete member with CMID \"{cmid}\".", cmid);
                // Let client know something is wrong.
                return null;
            }
        }

        public override async Task<MemberAuthenticationResultView> LoginSteam(string steamId, string authToken, string machineId)
        {
            try
            {
                var member = default(Member);

                if (_env.IsDevelopment() && !_auths.Add(steamId))
                {
                    // If member has already logged previously and running in a
                    // development environment, force creation of a new account.
                    //
                    // NOTE: 
                    // This is mostly for ease of development when launching
                    // multiple client instances from the same machine with the
                    // same steam ID.
                    // 
                    // Since a new account is created, a new CMID is assigned to it
                    // which means we don't have to deal with clients having the same
                    // CMID on the realtime servers.
                    member = null;
                    steamId = DateTime.UtcNow.ToBinary().ToString();
                }
                else
                {
                    // Otherwise; retrieve the Member instance from the database.
                    member = await _database.Members.FindAsync(steamId);
                }

                // If member does not exist, create a new member associated
                // with the steam ID provided.
                if (member == null)
                {
                    member = await CreateMember(steamId);

                    // If could not create member, means 'steamId' was already
                    // associated with another member.
                    if (member == null)
                        return new MemberAuthenticationResultView(MemberAuthenticationResult.InvalidEsns);
                }

                return await LoginMember(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(Events.Login, ex, "Failed to log member \"{steamId}\" in.", steamId);
                // Let client know something is wrong.
                return null;
            }
        }

        private async Task<MemberAuthenticationResultView> LoginMember(Member member)
        {
            // TODO: Consider replay attacks.

            // If member is banned; bail out with result = 'IsBanned'.
            if (member.IsBanned())
                return new MemberAuthenticationResultView(MemberAuthenticationResult.IsBanned);

            // Create new session for member login.
            var authToken = await _sessions.CreateSessionAsync(member);

            // Create views from models.
            var memberView = new MemberView().From(member);
            var memberStatsView = new PlayerStatisticsView().From(member);

            _logger.LogInformation(Events.Login, "\"{steamId}\" logged in.", member.SteamId);

            return new MemberAuthenticationResultView
            {
                MemberAuthenticationResult = MemberAuthenticationResult.Ok,

                AuthToken = authToken,
                MemberView = memberView,
                PlayerStatisticsView = memberStatsView,
                IsAccountComplete = member.EmailStatus == EmailAddressStatus.Verified,
                ServerTime = DateTime.UtcNow,

                LuckyDraw = default // Not used.
            };
        }

        // Attempts to create a new member with the specified 'steamId' and
        // insert into the database; returns null if member with 'steamId'
        // already exists.
        private async Task<Member> CreateMember(string steamId)
        {
            // Check if a member with the 'steamId' already exist in the
            // database, return null if so.
            if (await _database.Members.SteamIdExists(steamId))
            {
                _logger.LogInformation(Events.CreateAccount, "\"{steamId}\" already exist; cannot create account.", steamId);
                return null;
            }

            _logger.LogInformation(Events.CreateAccount, "\"{steamId}\" creating new account.", steamId);

            var member = new Member
            {
                Id = default,

                ClanId = null,
                SteamId = steamId,
                Name = null,
                Level = 1,
                MuteExpiration = null,
                BanExpiration = null,
                Credits = _accountConfig.Credits,
                Points = _accountConfig.Points,
                AccessLevel = MemberAccessLevel.Default,
                EmailStatus = EmailAddressStatus.Unverified,

                Transactions = new MemberTransactions
                {
                    Items = new List<ItemTransaction>(),
                    Points = new List<PointsDeposit>()
                },
                Socials = new MemberSocials
                {
                    IncomingRequests = new Dictionary<int, ContactRequest>(),
                    Contacts = new List<int>()
                },
                Inventory = new MemberInventory
                {
                    Items = new Dictionary<int, InventoryItem>()
                },
                Loadout = new MemberLoadout(),
                Statistics = new MemberStatistics()
            };

            member.Transactions.Points.Insert(0, new PointsDeposit
            {
                Date = DateTime.UtcNow,
                Points = member.Points,
                Type = PointsDepositType.Registration
            });

            // Assign default items.
            int GetDefaultItemId(string prefabName)
            {
                var item = _items.GetItem(prefabName);
                if (item == null)
                    throw new InvalidOperationException($"items.json does not contain a definition for default item \"{prefabName}\".");
                member.Inventory.Items.Add(item.Id, new InventoryItem { ItemId = item.Id, AmountRemaining = -1 });
                return item.Id;
            }

            // NOTE: 
            // The client did not configure LutzDefaultGearFace as a default item?
            // member.Loadout.GearFace = GetDefaultItemId("LutzDefaultGearFace");

            member.Loadout.GearBoots = GetDefaultItemId("LutzDefaultGearBoots");
            member.Loadout.GearHead = GetDefaultItemId("LutzDefaultGearHead");
            member.Loadout.GearUpperBody = GetDefaultItemId("LutzDefaultGearUpperBody");
            member.Loadout.GearLowerBody = GetDefaultItemId("LutzDefaultGearLowerBody");
            member.Loadout.GearGloves = GetDefaultItemId("LutzDefaultGearGloves");

            // Insert new member into database.
            await _database.Members.InsertAsync(member);

            return member;
        }

        // Completes the specified member by settings its email status to
        // verified, changes its name to the desired name and assigns the
        // default items.
        private async Task<Dictionary<int, int>> CompleteMember(Member member, string name)
        {
            Debug.Assert(member.EmailStatus != EmailAddressStatus.Verified);

            // Assign desired name.
            member.Name = name;
            member.EmailStatus = EmailAddressStatus.Verified;

            // Assign items.
            var itemsAttributed = new Dictionary<int, int>();
            foreach (var itemPrefabName in _accountConfig.Items)
            {
                var item = _items.GetItem(itemPrefabName);
                if (item == null)
                {
                    _logger.LogWarning(
                        Events.CreateAccount, 
                        "Item \"{itemPrefabName}\" does not exist in the ItemsManager. Check items.json and account.json.", 
                        itemPrefabName);
                }
                else
                {
                    var inventoryItem = new InventoryItem
                    {
                        ItemId = item.Id,

                        // TODO: Make configurable as well.
                        AmountRemaining = -1,
                        Expiration = null
                    };

                    if (!member.Inventory.Items.ContainsKey(item.Id))
                        member.Inventory.Items.Add(item.Id, inventoryItem);
                    itemsAttributed.Add(inventoryItem.ItemId, /* Not used. */ default);
                }
            }

            await _database.Members.UpdateAsync(member);

            return itemsAttributed;
        }
    }
}
