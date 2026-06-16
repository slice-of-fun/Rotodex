using System.Collections.Generic;

namespace Roto.Core;

public sealed class EventVarGroup(EventVarType type)
{
    public readonly EventVarType Type = type;
    public readonly List<EventVar> Vars = [];
}
