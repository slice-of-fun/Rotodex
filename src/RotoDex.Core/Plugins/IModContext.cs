using RotoDex.Core.Managers;

namespace RotoDex.Core.Plugins
{
    public interface IModContext
    {
        SaveManager SaveManager { get; }
        
        void Log(string message);
    }
}
