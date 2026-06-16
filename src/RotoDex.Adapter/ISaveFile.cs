using System.Collections.Generic;

namespace RotoDex.Adapter;

public interface ISaveFile
{
    string GameName { get; }
    int BoxCount { get; }
    
    IEnumerable<IPokemon> GetBox(int boxIndex);
    void SetBoxPokemon(int boxIndex, int slotIndex, IPokemon pokemon);
    
    byte[] Write();
}
