using System;
using Microsoft.Maui.Controls;
using Roto.Core;

namespace RotoDex.Mobile;

public partial class PokemonEditorPage : ContentPage
{
    private PKM _pokemon;

    public PokemonEditorPage(PKM pokemon)
    {
        InitializeComponent();
        _pokemon = pokemon;
        LoadPokemonData();
    }

    private void LoadPokemonData()
    {
        if (_pokemon == null) return;

        NicknameEntry.Text = _pokemon.Nickname;
        SpeciesEntry.Text = _pokemon.Species.ToString();
        LevelEntry.Text = _pokemon.CurrentLevel.ToString();
        
        OTEntry.Text = _pokemon.OriginalTrainerName;
        GameEntry.Text = _pokemon.Version.ToString();
        
        NatureEntry.Text = _pokemon.Nature.ToString();
    }

    private async void OnSaveChangesClicked(object sender, EventArgs e)
    {
        if (_pokemon == null) return;

        try
        {
            _pokemon.Nickname = NicknameEntry.Text;
            
            if (int.TryParse(SpeciesEntry.Text, out int species))
                _pokemon.Species = (ushort)species;
                
            if (int.TryParse(LevelEntry.Text, out int level))
                _pokemon.CurrentLevel = (byte)level;
                
            _pokemon.OriginalTrainerName = OTEntry.Text;
            
            if (int.TryParse(GameEntry.Text, out int game))
                _pokemon.Version = (GameVersion)game;
                
            if (int.TryParse(NatureEntry.Text, out int nature))
                _pokemon.Nature = (Nature)nature;

            await DisplayAlert("Success", "Pokémon updated successfully!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save changes: {ex.Message}", "OK");
        }
    }

    private async void OnImportClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                var bytes = await System.IO.File.ReadAllBytesAsync(result.FullPath);
                var imported = EntityFormat.GetFromBytes(bytes);
                
                if (imported != null)
                {
                    imported.Data.CopyTo(_pokemon.Data);
                    LoadPokemonData();
                    await DisplayAlertAsync("Imported", "Pokémon successfully imported! Press Save Changes to apply.", "OK");
                }
                else
                {
                    await DisplayAlertAsync("Error", "Invalid Pokémon file.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to import: {ex.Message}", "OK");
        }
    }

    private async void OnExportClicked(object sender, EventArgs e)
    {
        try
        {
            var bytes = _pokemon.Data.ToArray();
            string fileName = $"{_pokemon.Species}_{_pokemon.Nickname}.pk{_pokemon.Format}";
            
            string tempFile = System.IO.Path.Combine(FileSystem.CacheDirectory, fileName);
            await System.IO.File.WriteAllBytesAsync(tempFile, bytes);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Export Pokémon",
                File = new ShareFile(tempFile)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to export: {ex.Message}", "OK");
        }
    }

    private Task DisplayAlertAsync(string title, string message, string cancel)
    {
        return MainThread.InvokeOnMainThreadAsync(() => DisplayAlert(title, message, cancel));
    }
}
