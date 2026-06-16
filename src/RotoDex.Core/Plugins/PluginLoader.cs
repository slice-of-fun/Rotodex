using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RotoDex.Core.Plugins
{
    public class PluginLoader
    {
        private readonly List<IRotoMod> _loadedMods = new List<IRotoMod>();
        public IReadOnlyList<IRotoMod> LoadedMods => _loadedMods;

        public void LoadPlugins(string directoryPath, IModContext context)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                return;
            }

            var files = Directory.GetFiles(directoryPath)
                .Where(f => f.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || 
                            f.EndsWith(".rotomod", StringComparison.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    var modTypes = assembly.GetTypes()
                        .Where(t => typeof(IRotoMod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var type in modTypes)
                    {
                        if (Activator.CreateInstance(type) is IRotoMod mod)
                        {
                            mod.Initialize(context);
                            _loadedMods.Add(mod);
                            context.Log($"Successfully loaded mod: {mod.Name} v{mod.Version} by {mod.Author}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    context.Log($"Failed to load mod from {Path.GetFileName(file)}: {ex.Message}");
                }
            }
        }
    }
}
