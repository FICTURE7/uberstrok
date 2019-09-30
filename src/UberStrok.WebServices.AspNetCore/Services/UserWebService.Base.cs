using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.AspNetCore
{
    public abstract class BaseUserWebService : IUserAsyncWebServiceContract
    {
        public abstract Task<MemberOperationResult> ChangeMemberName(string authToken, string name, string locale, string machineId);
        public abstract Task<bool> IsDuplicateMemberName(string username);
        public abstract Task<List<string>> GenerateNonDuplicatedMemberNames(string username);
        public abstract Task<List<ItemInventoryView>> GetInventory(string authToken);
        public abstract Task<LoadoutView> GetLoadout(string authToken);
        public abstract Task<MemberOperationResult> SetLoadout(string authToken, LoadoutView loadoutView);
        public abstract Task<UberstrikeUserView> GetMember(string authToken);
        public abstract Task<MemberWalletView> GetMemberWallet(string authToken);
        public abstract Task<ItemTransactionPageView> GetItemTransactions(string authToken, int pageIndex, int elementPerPage);
        public abstract Task<PointsDepositPageView> GetPointsDeposits(string authToken, int pageIndex, int elementPerPage);

        async Task<byte[]> IUserAsyncWebServiceContract.ChangeMemberName(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var name = StringProxy.Deserialize(bytes);
                var locale = StringProxy.Deserialize(bytes);
                var machineId = StringProxy.Deserialize(bytes);
                var view = await ChangeMemberName(authToken, name, locale, machineId);
                using (var outBytes = new MemoryStream())
                {
                    EnumProxy<MemberOperationResult>.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IUserAsyncWebServiceContract.IsDuplicateMemberName(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var username = StringProxy.Deserialize(bytes);
                var view = await IsDuplicateMemberName(username);
                using (var outBytes = new MemoryStream())
                {
                    BooleanProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IUserAsyncWebServiceContract.GenerateNonDuplicatedMemberNames(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var username = StringProxy.Deserialize(bytes);
                var view = await GenerateNonDuplicatedMemberNames(username);
                using (var outBytes = new MemoryStream())
                {
                    ListProxy<string>.Serialize(outBytes, view, StringProxy.Serialize);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IUserAsyncWebServiceContract.GetInventory(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var view = await GetInventory(authToken);
                using (var outBytes = new MemoryStream())
                {
                    ListProxy<ItemInventoryView>.Serialize(outBytes, view, ItemInventoryViewProxy.Serialize);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IUserAsyncWebServiceContract.GetItemTransactions(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var pageIndex = Int32Proxy.Deserialize(bytes);
                var elementPerPage = Int32Proxy.Deserialize(bytes);
                var view = await GetItemTransactions(authToken, pageIndex, elementPerPage);
                using (var outBytes = new MemoryStream())
                {
                    ItemTransactionPageViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IUserAsyncWebServiceContract.GetLoadout(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var view = await GetLoadout(authToken);
                using (var outBytes = new MemoryStream())
                {
                    LoadoutViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IUserAsyncWebServiceContract.GetMember(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var view = await GetMember(authToken);
                using (var outBytes = new MemoryStream())
                {
                    UberstrikeUserViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IUserAsyncWebServiceContract.GetMemberWallet(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var view = await GetMemberWallet(authToken);
                using (var outBytes = new MemoryStream())
                {
                    MemberWalletViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IUserAsyncWebServiceContract.GetPointsDeposits(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var pageIndex = Int32Proxy.Deserialize(bytes);
                var elementPerPage = Int32Proxy.Deserialize(bytes);
                var view = await GetPointsDeposits(authToken, pageIndex, elementPerPage);
                using (var outBytes = new MemoryStream())
                {
                    PointsDepositPageViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IUserAsyncWebServiceContract.SetLoadout(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var loadout = LoadoutViewProxy.Deserialize(bytes);
                var view = await SetLoadout(authToken, loadout);
                using (var outBytes = new MemoryStream())
                {
                    EnumProxy<MemberOperationResult>.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        Task<byte[]> IUserAsyncWebServiceContract.GetMemberListSessionData(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IUserAsyncWebServiceContract.GetMemberListSessionData));

        Task<byte[]> IUserAsyncWebServiceContract.GetMemberSessionData(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IUserAsyncWebServiceContract.GetMemberSessionData));

        Task<byte[]> IUserAsyncWebServiceContract.GetCurrencyDeposits(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IUserAsyncWebServiceContract.GetCurrencyDeposits));

        Task<byte[]> IUserAsyncWebServiceContract.GetLoadoutServer(byte[] data)
            // TODO: Review if we need this.
            => throw new NotImplementedException();
    }
}
