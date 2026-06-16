using RotoDex.Adapter;

namespace RotoDex.Core.Managers
{
    public class SaveContext
    {
        public ISaveFile SaveFile { get; }
        public string FilePath { get; }
        public string DisplayName { get; }
        public RotoDex.Core.Commands.CommandManager CommandManager { get; }

        public SaveContext(ISaveFile saveFile, string filePath)
        {
            SaveFile = saveFile;
            FilePath = filePath;
            DisplayName = System.IO.Path.GetFileName(filePath);
            CommandManager = new RotoDex.Core.Commands.CommandManager();
        }
    }
}
