using System;

namespace RotoDex.Core.Resources.Models;

public record LearnsetModel(
    int Index,
    ushort[] Moves,
    byte[] Levels
);
