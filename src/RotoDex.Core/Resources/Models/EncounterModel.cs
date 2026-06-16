using System;

namespace RotoDex.Core.Resources.Models;

public record EncounterModel(
    ushort Species,
    byte Form,
    byte Level,
    string OT_Name,
    ushort TID,
    ushort SID,
    byte Ball,
    bool IsShiny,
    ushort[] Moves
);
