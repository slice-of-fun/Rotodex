namespace RotoDex.Core.Plugins
{
    public interface IRotoMod
    {
        string Name { get; }
        string Version { get; }
        string Author { get; }
        string Description { get; }

        void Initialize(IModContext context);
    }
}
