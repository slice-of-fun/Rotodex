using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Roto.Core;

namespace RotoDex.Web.Services;

/// <summary>
/// Manages the RotoDex Vault — a persistent, browser-local archive of Pokémon.
/// Persistence is backed by localStorage via JS interop.
/// </summary>
public class VaultService
{
    private const string StorageKey = "rotodex_vault_v1";

    private readonly IJSRuntime _js;
    private List<VaultEntry> _entries = new();
    private bool _loaded = false;

    public VaultService(IJSRuntime js)
    {
        _js = js;
    }

    public IReadOnlyList<VaultEntry> Entries => _entries;

    public event Action? OnChange;

    /// <summary>Load entries from localStorage. Must be called before first use.</summary>
    public async Task EnsureLoadedAsync()
    {
        if (_loaded) return;
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(json))
                _entries = JsonSerializer.Deserialize<List<VaultEntry>>(json) ?? new();
        }
        catch
        {
            _entries = new();
        }
        _loaded = true;
    }

    /// <summary>Deposit a PKM into the vault.</summary>
    public async Task DepositAsync(PKM pkm, string collection = "Default")
    {
        await EnsureLoadedAsync();

        var entry = new VaultEntry
        {
            Base64Data = Convert.ToBase64String(pkm.Data),
            Format = pkm.Extension,
            Species = pkm.Species,
            SpeciesName = ((Species)pkm.Species).ToString(),
            Nickname = pkm.Nickname,
            OT = pkm.OriginalTrainerName,
            Level = pkm.CurrentLevel,
            Collection = collection,
            DateAdded = DateTime.UtcNow
        };

        _entries.Add(entry);
        await PersistAsync();
        OnChange?.Invoke();
    }

    /// <summary>Remove a vault entry by ID.</summary>
    public async Task RemoveAsync(string id)
    {
        _entries.RemoveAll(e => e.Id == id);
        await PersistAsync();
        OnChange?.Invoke();
    }

    /// <summary>Return entries optionally filtered by collection and/or search text.</summary>
    public IEnumerable<VaultEntry> Query(string search = "", string collection = "")
    {
        var q = _entries.AsEnumerable();
        if (!string.IsNullOrEmpty(collection))
            q = q.Where(e => e.Collection.Equals(collection, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(search))
            q = q.Where(e =>
                e.SpeciesName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                e.Nickname.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                e.OT.Contains(search, StringComparison.OrdinalIgnoreCase));
        return q.OrderByDescending(e => e.DateAdded);
    }

    public IEnumerable<string> Collections => _entries
        .Select(e => e.Collection)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(c => c);

    private async Task PersistAsync()
    {
        var json = JsonSerializer.Serialize(_entries);
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }
}
