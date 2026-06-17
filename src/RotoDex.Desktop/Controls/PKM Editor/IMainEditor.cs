using System;
using Roto.Core;

namespace RotoDex.Desktop.Controls;

public interface IMainEditor : IPKMView
{
    PKM Entity { get; }
    SaveFile RequestSaveFile { get; }

    void UpdateIVsGB(bool skipForm);
    void ChangeNature(Nature newNature);
}

[Flags]
public enum UpdateLegalityArgs
{
    None,
    SkipMoveRepopulation = 1 << 0,
}
