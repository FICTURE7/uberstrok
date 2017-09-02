using Newtonsoft.Json;
using System;
using System.IO;
using UbzStuff.Core.Views;

namespace UbzStuff.WebServices.Db
{
    public class ProfileDb
    {
        private const string ROOT_DIR = "data/profiles";

        public ProfileDb()
        {
            if (!Directory.Exists(ROOT_DIR))
                Directory.CreateDirectory(ROOT_DIR);
        }

        public PublicProfileView Load(int cmid)
        {
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            var path = Path.Combine(ROOT_DIR, cmid + ".json");
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<PublicProfileView>(json);
        }

        public void Save(PublicProfileView view)
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
