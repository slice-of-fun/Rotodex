using System;

namespace Roto.Core;

public abstract class MyItem(SaveFile SAV, Memory<byte> raw) : SaveBlock<SaveFile>(SAV, raw);
