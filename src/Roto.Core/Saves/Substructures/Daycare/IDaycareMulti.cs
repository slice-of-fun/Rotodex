namespace Roto.Core;

public interface IDaycareMulti
{
    int DaycareCount { get; }

    IDaycareStorage this[int index] { get; }
}
