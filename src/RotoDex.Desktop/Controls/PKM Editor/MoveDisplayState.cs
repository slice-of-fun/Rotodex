using System.Drawing;
using System.Runtime.CompilerServices;
using Roto.Core;
using RotoDex.Drawing.PokeSprite.Properties;

namespace RotoDex.Desktop.Controls;

public static class MoveDisplayState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitmap? GetMoveImage(bool isIllegal, PKM pk, int index)
    {
        if (isIllegal)
            return Resources.warn;

        if (MoveInfo.IsDummiedMove(pk, index))
            return Resources.hint;

        return null;
    }
}
