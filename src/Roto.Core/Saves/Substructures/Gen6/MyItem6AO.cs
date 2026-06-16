using System;

namespace Roto.Core;

public sealed class MyItem6AO : MyItem
{
    public MyItem6AO(SAV6AO SAV,     Memory<byte> raw) : base(SAV, raw) { }
    public MyItem6AO(SAV6AODemo SAV, Memory<byte> raw) : base(SAV, raw) { }
}
