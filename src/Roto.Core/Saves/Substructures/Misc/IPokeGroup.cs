using System.Collections.Generic;

namespace Roto.Core;

public interface IPokeGroup
{
    IEnumerable<PKM> Contents { get; }
}
