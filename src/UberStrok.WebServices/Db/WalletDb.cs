using Newtonsoft.Json;
using System;
using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.Db
{
    public class WalletDb
    {
        private const string ROOT_DIR = "data/wallets";

        public WalletDb()
        {
            if (!Directory.Exists(ROOT_DIR))
                Directory.CreateDirectory(ROOT_DIR);
        }

        public MemberWalletView Load(int cmid)
        {
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            var path = Path.Combine(ROOT_DIR, cmid + ".json");
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<MemberWalletView>(json);
        }

        public void Save(MemberWalletView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            var cmid = view.Cmid;
            if (cmid <= 0)
                throw new ArgumentException("CMID of PublicProfileView must be greater than 0.");

            var path = Path.Combine(ROOT_DIR, cmid + ".json");
            var json = JsonConvert.SerializeObject(view);
            File.WriteAllText(path, json);
        }
    }
}
