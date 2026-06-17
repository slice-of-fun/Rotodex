using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Roto.Core;

namespace RotoDex.Desktop;

[JsonSerializable(typeof(RotoDexSettings))]
public sealed partial class RotoDexSettingsContext : JsonSerializerContext;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class RotoDexSettings : IProgramSettings
{
    public StartupSettings Startup { get; set; } = new();
    public BackupSettings Backup { get; set; } = new();
    public LocalResourceSettings LocalResources { get; set; } = new();

    // General
    public LegalitySettings Legality { get; set; } = new();
    public EntityConverterSettings Converter { get; set; } = new();
    public SetImportSettings Import { get; set; } = new();
    public SlotWriteSettings SlotWrite { get; set; } = new();
    public PrivacySettings Privacy { get; set; } = new();
    public SaveLanguageSettings SaveLanguage { get; set; } = new();

    // UI Tweaks
    public DisplaySettings Display { get; set; } = new();
    public SpriteSettings Sprite { get; set; } = new();
    public SoundSettings Sounds { get; set; } = new();
    public HoverSettings Hover { get; set; } = new();
    public BattleTemplateSettings BattleTemplate { get; set; } = new();

    // GUI Specific
    public DrawConfig Draw { get; set; } = new();
    public AdvancedSettings Advanced { get; set; } = new();
    public EntityEditorSettings EntityEditor { get; set; } = new();
    public EntityDatabaseSettings EntityDb { get; set; } = new();
    public EncounterDatabaseSettings EncounterDb { get; set; } = new();
    public MysteryGiftDatabaseSettings MysteryDb { get; set; } = new();
    public ReportGridSettings Report { get; set; } = new();

    [Browsable(false)]
    public SlotExportSettings SlotExport { get; set; } = new();

    [Browsable(false), JsonIgnore]
    IStartupSettings IProgramSettings.Startup => Startup;

    private static RotoDexSettingsContext GetContext() => new(new()
    {
        WriteIndented = true,
        Converters = { new ColorJsonConverter() },
    });

    public sealed class ColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => ColorTranslator.FromHtml(reader.GetString() ?? string.Empty);

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) => writer.WriteStringValue($"#{value.R:x2}{value.G:x2}{value.B:x2}");
    }

    public static RotoDexSettings GetSettings(string configPath)
    {
        if (!File.Exists(configPath))
            return new RotoDexSettings();

        try
        {
            var lines = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize(lines, GetContext().RotoDexSettings) ?? new RotoDexSettings();
        }
        catch (Exception x)
        {
            DumpConfigError(x);
            return new RotoDexSettings();
        }
    }

    public static async Task SaveSettings(string configPath, RotoDexSettings cfg)
    {
        try
        {
            // Serialize the object asynchronously and write it to the path.
            await using var fs = File.Create(configPath);
            await JsonSerializer.SerializeAsync(fs, cfg, GetContext().RotoDexSettings).ConfigureAwait(false);
        }
        catch (Exception x)
        {
            DumpConfigError(x);
        }
    }

    private static async void DumpConfigError(Exception x)
    {
        try
        {
            await File.WriteAllTextAsync("config error.txt", x.ToString());
        }
        catch (Exception)
        {
            Debug.WriteLine(x); // ???
        }
    }
}
