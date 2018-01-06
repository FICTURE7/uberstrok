using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.Db
{
    public class InventoryDb
    {
        private const string ROOT_DIR = "data/inventories";

        public InventoryDb()
        {
            if (!Directory.Exists(ROOT_DIR))
                Directory.CreateDirectory(ROOT_DIR);
        }

        public List<ItemInventoryView> Load(int cmid)
        {
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            var path = Path.Combine(ROOT_DIR, cmid + ".json");
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<ItemInventoryView>>(json);
        }

        public void Save(int cmid, List<ItemInventoryView> view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            var path = Path.Combine(ROOT_DIR, cmid + ".json");
            var json = JsonConvert.SerializeObject(view);
            File.WriteAllText(path, json);
        }
    }
}
