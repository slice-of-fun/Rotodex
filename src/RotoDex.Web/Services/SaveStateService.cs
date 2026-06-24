using Roto.Core;

namespace RotoDex.Web.Services;

/// <summary>
/// Singleton state service holding the currently loaded save file across pages.
/// </summary>
public class SaveStateService
{
    public SaveFile? LoadedSav { get; private set; }

    public event Action? OnChange;

    public void SetSave(SaveFile sav)
    {
        LoadedSav = sav;
        NotifyStateChanged();
    }

    public void Clear()
    {
        LoadedSav = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
