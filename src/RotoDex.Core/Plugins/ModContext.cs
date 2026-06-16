using System;
using RotoDex.Core.Managers;

namespace RotoDex.Core.Plugins
{
    public class ModContext : IModContext
    {
        public SaveManager SaveManager { get; }

        public ModContext(SaveManager saveManager)
        {
            SaveManager = saveManager ?? throw new ArgumentNullException(nameof(saveManager));
        }

        public void Log(string message)
        {
            // Simple console logging for now, could be written to a mod log file.
            Console.WriteLine($"[RotoMod] {message}");
        }
    }
}
