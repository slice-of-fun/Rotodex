namespace Roto.Core;

/// <summary>
/// Interface for Pokémon with a GCRegion.
/// </summary>
/// <remarks>Used by Colosseum/XD to indicate string encoding format.</remarks>
public interface IGCRegion
{
    GCRegion CurrentRegion { get; set; }
    GCRegion OriginalRegion { get; set; }
}
