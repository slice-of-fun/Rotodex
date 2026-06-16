namespace RotoDex.Core.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
