using System;
using System.Diagnostics;
using System.IO;

namespace UberStrok.Patcher
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            const int UBERSTRIKE_STEAMAPP_ID = 291210;

            var sw = Stopwatch.StartNew();
            Console.WriteLine(" Searching for Steam installation...");

            var steamPath = default(string);
            try { steamPath = Steam.Path; }
            catch (DirectoryNotFoundException)
            {
                Console.Error.WriteLine(" Unable to find Steam installation.");
                return 1;
            }

            Console.WriteLine(" -----------------------------------");
            Console.WriteLine(" Path -> " + steamPath);
            Console.WriteLine(" Games ->");
            foreach (var keyValue in Steam.Apps)
            {
                var app = keyValue.Value;
                int id = app.Id;
                string name = app.Name;

                Console.WriteLine(id.ToString().PadLeft(" Games ->".Length) + " -> " + name);
            }
            Console.WriteLine(" -----------------------------------");
            Console.WriteLine(" Searching for UberStrike installation...");

            var uberStrikeApp = default(SteamApp);
            if (!Steam.Apps.TryGetValue(UBERSTRIKE_STEAMAPP_ID, out uberStrikeApp))
            {
                Console.Error.WriteLine(" Unable to find UberStrike manifest.");
                return 1;
            }

            var uberStrikePath = default(string);
            try { uberStrikePath = uberStrikeApp.Path; }
            catch
            {
                Console.Error.WriteLine(" Unable to parse UberStrike manifest.");
                return 1;
            }

            if (!Directory.Exists(uberStrikePath))
            {
                Console.Error.WriteLine(" Unable to find UberStrike installation directory.");
                return 1;
            }

            Console.WriteLine(" -----------------------------------");
            Console.WriteLine(" Path -> " + uberStrikePath);

            var uberStrike = new UberStrike(uberStrikePath);

            /*
            Console.WriteLine(" Backups ->");

            var dlls = Directory.GetFiles(uberStrike.ManagedPath, "*.dll");
            Directory.CreateDirectory(Path.Combine(uberStrike.ManagedPath, "backup"));
            foreach (var dll in dlls)
            {
                var fileName = Path.GetFileName(dll);
                var dst = Path.Combine(uberStrike.ManagedPath, "backup", fileName);

                File.Copy(dll, dst, true);

                Console.WriteLine(new string(' ', " Games ->".Length) + fileName + " -> " + "backup/" + fileName);
            }

            Console.WriteLine(" -----------------------------------");
            */

            var patches = new Patch[]
            {
                new QuickSwitchPatch(),
                new WebServicesPatch(),
                new HostPatch()
            };

            Console.WriteLine(" Patches ->");
            foreach (var patch in patches)
            {
                var name = patch.GetType().Name;
                Console.Write(new string(' ', " Games ->".Length) + "applying " + name + " -> ");

                try
                {
                    patch.Apply(uberStrike);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("done");
                    Console.ResetColor();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("failed");
                    Console.ResetColor();
                }
            }

            Console.WriteLine(" Writing new assemblies...");
            try { uberStrike.Save("patched"); }
            catch { Console.Error.WriteLine("Failed to write."); }

            sw.Stop();

            Console.WriteLine(" -----------------------------------");
            Console.WriteLine($" Finished in {sw.Elapsed.TotalMilliseconds}ms");
            return 0;
        }
    }
}
