using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.IO;

namespace UberStrok.Patcher
{
    public class UberStrike
    {
        private readonly string _managedPath;
        private readonly ModuleContext _moduleCtx;
        private readonly ModuleDefMD _assemblyCSharp;
        private readonly ModuleDefMD _assemblyCSharpFirstpass;
        private readonly ModuleDefMD _unityEngine;

        private readonly Dictionary<string, ModuleDefMD> _modules;

        public UberStrike(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var resolver = new AssemblyResolver();
            var moduleCtx = new ModuleContext(resolver);
            resolver.DefaultModuleContext = moduleCtx;
            resolver.EnableTypeDefCache = true;

            _moduleCtx = moduleCtx;

            _modules = new Dictionary<string, ModuleDefMD>();
            _managedPath = Path.Combine(path, "UberStrike_Data", "Managed");

            _assemblyCSharp = GetModule("Assembly-CSharp.dll");
            _assemblyCSharpFirstpass = GetModule("Assembly-CSharp-firstpass.dll");
            _unityEngine = GetModule("UnityEngine.dll");
        }

        public ModuleDefMD AssemblyCSharp => _assemblyCSharp;
        public ModuleDefMD AssemblyCSharpFirstpass => _assemblyCSharpFirstpass;
        public ModuleDefMD UnityEngine => _unityEngine;

        public string ManagedPath => _managedPath;

        public ModuleDefMD GetModule(string name)
        {
            var modulePath = Path.Combine(_managedPath, name);

            var module = default(ModuleDefMD);
            if (!_modules.TryGetValue(modulePath, out module))
            {
                module = ModuleDefMD.Load(modulePath, _moduleCtx);
                module.EnableTypeDefFindCache = true;

                _moduleCtx.AssemblyResolver.AddToCache(module);
                _modules.Add(modulePath, module);
            }

            return module;
        }

        public void Save(string outDir)
        {
            Directory.CreateDirectory(outDir);
            foreach (var keyValue in _modules)
            {
                var src = keyValue.Key;
                var dst = Path.Combine(outDir,  Path.GetFileName(src));

                keyValue.Value.Write(dst);
            }
        }
    }
}
