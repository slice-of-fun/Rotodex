using System;
using System.Text.Json.Serialization;

namespace RotoDex.Web.Services;

/// <summary>
/// A single Pokémon stored in the RotoDex Vault.
/// The raw PKM bytes are stored as Base64 so they can be persisted in localStorage.
/// </summary>
public class VaultEntry
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [JsonPropertyName("base64")]
    public string Base64Data { get; set; } = string.Empty;

    /// <summary>File extension / format tag (e.g. "pk9", "pk8", "pk6").</summary>
    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;

    [JsonPropertyName("species")]
    public int Species { get; set; }

    [JsonPropertyName("speciesName")]
    public string SpeciesName { get; set; } = string.Empty;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;

    [JsonPropertyName("ot")]
    public string OT { get; set; } = string.Empty;

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("collection")]
    public string Collection { get; set; } = "Default";

    [JsonPropertyName("dateAdded")]
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    // Convenience: raw bytes decoded from Base64Data
    [JsonIgnore]
    public byte[] Data => Convert.FromBase64String(Base64Data);
}
