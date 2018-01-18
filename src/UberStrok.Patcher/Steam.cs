using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace UberStrok.Patcher
{
    public static class Steam
    {
        private delegate string PathFinder();

        private readonly static Dictionary<int, SteamApp> _apps;
        private readonly static string _path;

        public static string Path => _path;
        public static Dictionary<int, SteamApp> Apps => _apps;

        static Steam()
        {
            _path = GetSteamPath();
            _apps = GetSteamApps();
        }

        private static Dictionary<int, SteamApp> GetSteamApps()
        {
            var apps = new Dictionary<int, SteamApp>();
            var steamAppsPath = System.IO.Path.Combine(Path, "SteamApps");
            if (steamAppsPath == null)
                throw new DirectoryNotFoundException("Unable to find Steam apps directory. " + steamAppsPath);

            var manifests = Directory.GetFiles(steamAppsPath, "appmanifest_*.acf");
            foreach (var manifest in manifests)
            {
                var idStart = manifest.IndexOf("_") + 1;
                var idEnd = manifest.IndexOf(".acf");

                var idStr = manifest.Substring(idStart, idEnd - idStart);
                var id = -1;
                if (!int.TryParse(idStr, out id))
                    continue;

                var steamApp = new SteamApp(manifest);
                apps.Add(id, steamApp);
            }

            return apps;
        }

        private static string GetSteamPath()
        {
            var finders = new PathFinder[]
            {
                () => {
                    /*
                        Find the steam install path by using the HKEY_CURRENT_USER\Software\Valve\Steam\SteamPath value.
                        Which does not require admin access.
                     */
                    using(var softwareReg = Registry.CurrentUser.OpenSubKey("software"))
                    using(var valveReg = softwareReg.OpenSubKey("valve"))
                    using(var steamReg = valveReg.OpenSubKey("steam"))
                    {
                       if (steamReg == null)
                            return null;

                        var steamPath = steamReg.GetValue("SteamPath") as string;
                        return steamPath;
                    }
                },
                () => {
                    /*
                        Find the steam install path by using the HKEY_LOCAL_MACHINE\Software\WOW6432Node\Valve\Steam\InstallPath value.
                        Which does **requires** admin access. I think.

                        This is for 64-bit.
                     */
                    using(var softwareReg = Registry.LocalMachine.OpenSubKey("software"))
                    using(var wow6432nodeReg = softwareReg.OpenSubKey("wow6432node"))
                    using(var valveReg = wow6432nodeReg.OpenSubKey("valve"))
                    using(var steamReg = valveReg.OpenSubKey("steam"))
                    {
                        if (steamReg == null)
                            return null;

                        var installPath = steamReg.GetValue("InstallPath") as string;
                        return installPath;
                    }
                },
                () => {
                    /*
                        Find the steam install path by using the HKEY_LOCAL_MACHINE\Software\WOW6432Node\Valve\Steam\InstallPath value.
                        Which does **requires** admin access. I think.

                        This is for 32-bit.
                     */
                    using(var softwareReg = Registry.LocalMachine.OpenSubKey("software"))
                    using(var valveReg = softwareReg.OpenSubKey("valve"))
                    using(var steamReg = valveReg.OpenSubKey("steam"))
                    {
                        if (steamReg == null)
                            return null;

                        var installPath = steamReg.GetValue("InstallPath") as string;
                        return installPath;
                    }
                },
                () => {
                    /*
                        Find the steam install path by manually looking for it in the program files

                        This is for 32-bit.
                     */
                    var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                    var steamPath = System.IO.Path.Combine(programFilesX86, "Steam");
                    if (!Directory.Exists(steamPath))
                        return null;

                    return steamPath;
                },
                () => {
                    /*
                        Find the steam install path by manually looking for it in the program files

                        This is for 64-bit.
                     */
                    var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    var steamPath = System.IO.Path.Combine(programFilesX86, "Steam");
                    if (!Directory.Exists(steamPath))
                        return null;

                    return steamPath;
                }
            };

            var path = default(string);
            foreach (var finder in finders)
            {
                try { path = finder(); }
                catch { continue; }

                if (path != null)
                    break;
            }

            if (path == null)
                throw new DirectoryNotFoundException("Unable to find steam installation directory path.");

            return System.IO.Path.GetFullPath(path);
        }
    }
}
