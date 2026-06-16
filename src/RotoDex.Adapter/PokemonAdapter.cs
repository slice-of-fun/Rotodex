using System;
using Roto.Core;

namespace RotoDex.Adapter;

public class PokemonAdapter : IPokemon
{
    private readonly PKM _pkm;

    public PokemonAdapter(PKM pkm)
    {
        _pkm = pkm ?? throw new ArgumentNullException(nameof(pkm));
    }

    public ushort Species 
    { 
        get => _pkm.Species; 
        set => _pkm.Species = value; 
    }

    public byte Level 
    { 
        get => _pkm.CurrentLevel; 
        set => _pkm.CurrentLevel = value; 
    }

    public string Nickname 
    { 
        get => _pkm.Nickname; 
        set => _pkm.Nickname = value; 
    }

    public int[] IVs 
    { 
        get => new[] { _pkm.IV_HP, _pkm.IV_ATK, _pkm.IV_DEF, _pkm.IV_SPA, _pkm.IV_SPD, _pkm.IV_SPE }; 
        set 
        {
            if (value?.Length == 6)
            {
                _pkm.IV_HP = value[0];
                _pkm.IV_ATK = value[1];
                _pkm.IV_DEF = value[2];
                _pkm.IV_SPA = value[3];
                _pkm.IV_SPD = value[4];
                _pkm.IV_SPE = value[5];
            }
        }
    }

    public byte[] EVs 
    { 
        get => new[] { (byte)_pkm.EV_HP, (byte)_pkm.EV_ATK, (byte)_pkm.EV_DEF, (byte)_pkm.EV_SPA, (byte)_pkm.EV_SPD, (byte)_pkm.EV_SPE }; 
        set 
        {
            if (value?.Length == 6)
            {
                _pkm.EV_HP = value[0];
                _pkm.EV_ATK = value[1];
                _pkm.EV_DEF = value[2];
                _pkm.EV_SPA = value[3];
                _pkm.EV_SPD = value[4];
                _pkm.EV_SPE = value[5];
            }
        }
    }

    public ushort[] Moves 
    { 
        get => new[] { _pkm.Move1, _pkm.Move2, _pkm.Move3, _pkm.Move4 }; 
        set 
        {
            if (value?.Length == 4)
            {
                _pkm.Move1 = value[0];
                _pkm.Move2 = value[1];
                _pkm.Move3 = value[2];
                _pkm.Move4 = value[3];
            }
        }
    }

    public bool IsShiny => _pkm.IsShiny;

    public int Gender 
    { 
        get => _pkm.Gender; 
        set => _pkm.Gender = (byte)value; 
    }

    public int Nature 
    { 
        get => (int)_pkm.Nature; 
        set => _pkm.Nature = (Nature)value; 
    }

    public byte[] GetRawData() => _pkm.Data.ToArray();
    
    public IPokemon Clone() => new PokemonAdapter(_pkm.Clone());
    
    internal PKM GetUnderlyingPkm() => _pkm;
}
