using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Roto.Core;

namespace RotoDex.Mobile;

public class IllegalPokemonViewModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Report { get; set; } = string.Empty;
    public PKM Pokemon { get; set; } = null!;
}

public partial class LegalityDashboardPage : ContentPage
{
    private readonly SaveFile _sav;
    public ObservableCollection<IllegalPokemonViewModel> IllegalResults { get; set; } = new();

    public LegalityDashboardPage(SaveFile sav)
    {
        InitializeComponent();
        _sav = sav;
        ResultsGrid.ItemsSource = IllegalResults;
    }

    private async void OnAnalyzeClicked(object? sender, EventArgs e)
    {
        AnalyzeBtn.IsEnabled = false;
        LoadingSpinner.IsVisible = true;
        LoadingSpinner.IsRunning = true;
        StatusLabel.Text = "Analyzing save file...";
        IllegalResults.Clear();

        // Run analysis on a background thread so we don't freeze the UI
        var results = await Task.Run(() => AnalyzeSave());

        foreach (var res in results)
        {
            IllegalResults.Add(res);
        }

        LoadingSpinner.IsVisible = false;
        LoadingSpinner.IsRunning = false;
        AnalyzeBtn.IsEnabled = true;

        if (IllegalResults.Count == 0)
        {
            StatusLabel.Text = "Analysis complete! 100% Legal.";
            StatusLabel.TextColor = Color.FromArgb("#03DAC6"); // Green-ish success
        }
        else
        {
            StatusLabel.Text = $"Analysis complete! Found {IllegalResults.Count} illegal Pokémon.";
            StatusLabel.TextColor = Color.FromArgb("#ff6b6b"); // Red warning
        }
    }

    private ObservableCollection<IllegalPokemonViewModel> AnalyzeSave()
    {
        var results = new ObservableCollection<IllegalPokemonViewModel>();

        for (int b = 0; b < _sav.BoxCount; b++)
        {
            var boxName = _sav is IBoxDetailName bn ? bn.GetBoxName(b) : $"Box {b + 1}";

            for (int s = 0; s < 30; s++)
            {
                var pkm = _sav.GetBoxSlotAtIndex(b, s);
                if (pkm.Species == 0) continue; // Skip empty

                var la = new LegalityAnalysis(pkm);
                if (!la.Valid)
                {
                    results.Add(new IllegalPokemonViewModel
                    {
                        DisplayName = pkm.Nickname,
                        Location = $"{boxName} - Slot {s + 1}",
                        Report = la.Report(),
                        Pokemon = pkm
                    });
                }
            }
        }

        return results;
    }

    private async void OnResultTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is PKM pkm)
        {
            await Navigation.PushAsync(new PokemonEditorPage(pkm));
        }
    }
}
