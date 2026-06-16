namespace Roto.Core;

public enum RanchOwnershipType : byte
{
    None              = 0,
    Trainer           = 1,
    Hayley            = 4,
    Hayley_Traded     = 5,
}

// this might actually be an index to which Mii trainer owns the Pokémon
public enum RanchOwnershipStatus : ushort
{
    None = 0,
    Traded = 2,
}
