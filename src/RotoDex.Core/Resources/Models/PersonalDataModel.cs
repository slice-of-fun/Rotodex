using System;

namespace RotoDex.Core.Resources.Models;

public record PersonalDataModel(
    ushort Species,
    byte Form,
    int HP,
    int ATK,
    int DEF,
    int SPA,
    int SPD,
    int SPE,
    byte Type1,
    byte Type2,
    ushort Ability1,
    ushort Ability2,
    ushort AbilityH
);
