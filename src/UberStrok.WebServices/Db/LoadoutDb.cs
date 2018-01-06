using Newtonsoft.Json;
using System;
using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.Db
{
    public class LoadoutDb
    {
        private const string ROOT_DIR = "data/loadouts";

        public LoadoutDb()
        {
            if (!Directory.Exists(ROOT_DIR))
                Directory.CreateDirectory(ROOT_DIR);
        }

        public LoadoutView Load(int cmid)
        {
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            var path = Path.Combine(ROOT_DIR, cmid + ".json");
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<LoadoutView>(json);
        }

        public void Save(LoadoutView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            var cmid = view.Cmid;
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            var path = Path.Combine(ROOT_DIR, cmid + ".json");
            var json = JsonConvert.SerializeObject(view);
            File.WriteAllText(path, json);
        }
    }
}
