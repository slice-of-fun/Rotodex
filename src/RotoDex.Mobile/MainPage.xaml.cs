using System.Diagnostics;
using Roto.Core;

namespace RotoDex.Mobile;

public partial class MainPage : ContentPage
{
    private SaveFile? _loadedSav;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnOpenSaveClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                StatusLabel.Text = $"Loading {result.FileName}...";
                OpenSaveBtn.IsEnabled = false;

                // Read bytes
                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                byte[] saveBytes = ms.ToArray();

                // Parse with Roto.Core
                var sav = SaveUtil.GetSaveFile(saveBytes);

                if (sav != null) // Simple check if it loaded successfully
                {
                    _loadedSav = sav;

                    // Success! Show details
                    SaveDetailsCard.IsVisible = true;
                    StatusLabel.Text = "Successfully loaded save file.";
                    
                    TrainerNameLabel.Text = sav.OT;
                    GameVersionLabel.Text = sav.Version.ToString();
                    PlayTimeLabel.Text = $"{sav.PlayedHours:D2}:{sav.PlayedMinutes:D2}";
                    
                    ViewBoxesBtn.IsEnabled = true;
                }
                else
                {
                    await DisplayAlertAsync("Invalid Save", "Roto.Core could not parse this save file. It may be corrupt or unsupported.", "OK");
                    StatusLabel.Text = "Failed to load save file.";
                    SaveDetailsCard.IsVisible = false;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await DisplayAlertAsync("Error", "An unexpected error occurred while loading the file.", "OK");
            StatusLabel.Text = "Error loading file.";
        }
        finally
        {
            OpenSaveBtn.IsEnabled = true;
        }
    }

    private async void OnExportSaveClicked(object sender, EventArgs e)
    {
        if (_loadedSav == null) return;

        try
        {
            var bytes = _loadedSav.Write().ToArray();
            string fileName = "exported_save.sav";
            
            string tempFile = System.IO.Path.Combine(FileSystem.CacheDirectory, fileName);
            await System.IO.File.WriteAllBytesAsync(tempFile, bytes);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Export Save File",
                File = new ShareFile(tempFile)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to export save: {ex.Message}", "OK");
        }
    }

    private Task DisplayAlertAsync(string title, string message, string cancel)
    {
        return MainThread.InvokeOnMainThreadAsync(() => DisplayAlert(title, message, cancel));
    }

    private async void OnViewBoxesClicked(object sender, EventArgs e)
    {
        if (_loadedSav == null) return;

        await Navigation.PushAsync(new BoxViewerPage(_loadedSav));
    }

    private async void OnLegalityCheckClicked(object sender, EventArgs e)
    {
        if (_loadedSav == null) return;

        await Navigation.PushAsync(new LegalityDashboardPage(_loadedSav));
    }
}
