using System;
using System.Collections.Generic;
using System.Linq;
using Roto.Core;

namespace RotoDex.Adapter;

public class SaveAdapter : ISaveFile
{
    private readonly SaveFile _save;

    public SaveAdapter(byte[] rawData)
    {
        _save = SaveUtil.GetSaveFile(rawData) ?? throw new ArgumentException("Unsupported save file format.");
    }

    public string GameName => _save.Version.ToString();
    public int BoxCount => _save.BoxCount;

    public IEnumerable<IPokemon> GetBox(int boxIndex)
    {
        var boxData = _save.GetBoxData(boxIndex);
        return boxData.Select(p => new PokemonAdapter(p)).Cast<IPokemon>();
    }

    public void SetBoxPokemon(int boxIndex, int slotIndex, IPokemon pokemon)
    {
        if (pokemon is PokemonAdapter adapter)
        {
            _save.SetBoxSlotAtIndex(adapter.GetUnderlyingPkm(), boxIndex, slotIndex);
        }
        else
        {
            throw new ArgumentException("Unsupported IPokemon implementation.", nameof(pokemon));
        }
    }

    public byte[] Write()
    {
        return _save.Write().ToArray();
    }
    
    internal SaveFile GetUnderlyingSave() => _save;
}
