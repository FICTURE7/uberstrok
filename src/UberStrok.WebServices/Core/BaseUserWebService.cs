using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Core
{
    public abstract class BaseUserWebService : BaseWebService, IUserWebServiceContract
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(BaseUserWebService).Name);

        protected BaseUserWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public abstract bool OnIsDuplicateMemberName(string username);
        public abstract MemberOperationResult OnSetLoaduout(string authToken, LoadoutView loadoutView);
        public abstract UberstrikeUserView OnGetMember(string authToken);
        public abstract LoadoutView OnGetLoadout(string authToken);
        public abstract LoadoutView OnGetLoadoutServer(string serviceAuth, string authToken);
        public abstract List<ItemInventoryView> OnGetInventory(string authToken);

        byte[] IUserWebServiceContract.ChangeMemberName(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle ChangeMemberName request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GenerateNonDuplicateMemberNames(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GenerateNonDuplicateMemberNames request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetCurrencyDeposits(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetCurrentDeposits request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetInventory(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var authToken = StringProxy.Deserialize(bytes);

                    var view = OnGetInventory(authToken);
                    using (var outBytes = new MemoryStream())
                    {
                        ListProxy<ItemInventoryView>.Serialize(outBytes, view, ItemInventoryViewProxy.Serialize);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetInventory request:");
                Log.Error(ex);
                return null;
            }
        }


        byte[] IUserWebServiceContract.GetItemTransactions(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetItemTransactions request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetLoadout(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var authToken = StringProxy.Deserialize(bytes);

                    LoadoutView view = OnGetLoadout(authToken);
                    using (var outBytes = new MemoryStream())
                    {
                        LoadoutViewProxy.Serialize(outBytes, view);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetLoadout request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetLoadoutServer(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var serviceAuth = StringProxy.Deserialize(bytes);
                    var authToken = StringProxy.Deserialize(bytes);

                    LoadoutView view = OnGetLoadoutServer(serviceAuth, authToken);
                    using (var outBytes = new MemoryStream())
                    {
                        LoadoutViewProxy.Serialize(outBytes, view);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetLoadout request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetMember(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var authToken = StringProxy.Deserialize(bytes);

                    var view = OnGetMember(authToken);
                    using (var outBytes = new MemoryStream())
                    {
                        UberstrikeUserViewProxy.Serialize(outBytes, view);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMember request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetMemberListSessionData(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMemberListSessionData request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetMemberSessionData(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMemberSessionData request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetMemberWallet(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMemberWallet request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.GetPointsDeposits(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetPointsDeposits request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.IsDuplicateMemberName(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var username = StringProxy.Deserialize(bytes);

                    var result = OnIsDuplicateMemberName(username);
                    using (var outBytes = new MemoryStream())
                    {
                        BooleanProxy.Serialize(outBytes, result);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle IsDuplicateMemberName request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IUserWebServiceContract.SetLoadout(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var authToken = StringProxy.Deserialize(bytes);
                    var loadoutView = LoadoutViewProxy.Deserialize(bytes);

                    var result = OnSetLoaduout(authToken, loadoutView);
                    using (var outBytes = new MemoryStream())
                    {
                        EnumProxy<MemberOperationResult>.Serialize(outBytes, result);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle SetLoadout request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
