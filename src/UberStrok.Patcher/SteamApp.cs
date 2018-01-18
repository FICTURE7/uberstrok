using System;
using System.IO;

namespace UberStrok.Patcher
{
    public class SteamApp
    {
        private bool _loaded;

        private int _id;
        private string _path;
        private string _name;
        private readonly string _manifestPath;

        public SteamApp(string manifestPath)
        {
            if (manifestPath == null)
                throw new ArgumentNullException(nameof(manifestPath));

            _loaded = false;
            _manifestPath = manifestPath;
        }

        public int Id
        {
            get
            {
                if (!_loaded)
                    Load();

                return _id;
            }
        }

        public string Path
        {
            get
            {
                if (!_loaded)
                    Load();

                return _path;
            }
        }

        public string Name
        {
            get
            {
                if (!_loaded)
                    Load();

                return _name;
            }
        }

        private void Load()
        {
            /*var keyValues = KeyValues.Parse(_manifestPath);*/

            var data = File.ReadAllText(_manifestPath);

            var appid = int.Parse(GetValue(data, "appid"));
            var path = GetValue(data, "installdir");
            var name = GetValue(data, "name");

            _id = appid;
            _path = System.IO.Path.Combine(Steam.Path, "SteamApps", "common", path);
            _name = name;
            _loaded = true;
        }

        private string GetValue(string keyValue, string key)
        {
            var actualKey = "\"" + key + "\"";
            var keyStart = keyValue.IndexOf(actualKey);
            var index = keyStart + actualKey.Length;
            var token = string.Empty;

            while (true)
            {
                if (index >= keyValue.Length)
                    break;

                var c = keyValue[index];
                if (char.IsWhiteSpace(c))
                {
                    index++;
                    continue;
                }

                if (c == '"')
                {
                    while (true)
                    {
                        c = keyValue[++index];
                        if (c == '"')
                            return token;

                        token += c;
                    }
                }
            }

            return null;
        }
    }
}
