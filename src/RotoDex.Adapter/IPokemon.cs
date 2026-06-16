using System;

namespace RotoDex.Adapter;

public interface IPokemon
{
    ushort Species { get; set; }
    byte Level { get; set; }
    string Nickname { get; set; }
    int[] IVs { get; set; }
    byte[] EVs { get; set; }
    ushort[] Moves { get; set; }
    bool IsShiny { get; }
    int Gender { get; set; }
    int Nature { get; set; }
    
    byte[] GetRawData();
    IPokemon Clone();
}
