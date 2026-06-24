using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Roto.Core;

namespace RotoDex.Mobile;

public class BoxSlotViewModel
{
    public int SlotIndex { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = "#00000000";
    public PKM Pokemon { get; set; } = null!;
}

public partial class BoxViewerPage : ContentPage
{
    private readonly SaveFile _sav;
    private int _currentBox;
    public ObservableCollection<BoxSlotViewModel> BoxSlots { get; set; } = new();

    public BoxViewerPage(SaveFile sav)
    {
        InitializeComponent();
        _sav = sav;
        _currentBox = _sav.CurrentBox;
        
        BoxGrid.ItemsSource = BoxSlots;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadBox(_currentBox);
    }

    private void LoadBox(int boxIndex)
    {
        BoxSlots.Clear();
        BoxNameLabel.Text = _sav is IBoxDetailName bn ? bn.GetBoxName(boxIndex) : $"Box {boxIndex + 1}";

        for (int i = 0; i < 30; i++) // Standard PC box is 30 slots
        {
            var pkm = _sav.GetBoxSlotAtIndex(boxIndex, i);
            
            var vm = new BoxSlotViewModel
            {
                SlotIndex = i,
                Pokemon = pkm
            };

            if (pkm.Species == 0) // Empty
            {
                vm.DisplayName = "Empty";
                vm.Details = "";
                vm.BackgroundColor = "#0dffffff"; // Faint transparent
            }
            else
            {
                vm.DisplayName = pkm.Nickname;
                vm.Details = $"Lv. {pkm.CurrentLevel}";
                vm.BackgroundColor = "#336200EE"; // Purple tint for populated
            }

            BoxSlots.Add(vm);
        }
    }

    private void OnPreviousBoxClicked(object sender, EventArgs e)
    {
        if (_currentBox > 0)
        {
            _currentBox--;
            LoadBox(_currentBox);
        }
    }

    private void OnNextBoxClicked(object sender, EventArgs e)
    {
        if (_currentBox < _sav.BoxCount - 1)
        {
            _currentBox++;
            LoadBox(_currentBox);
        }
    }

    private async void OnSlotTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is int slotIndex)
        {
            var slot = BoxSlots[slotIndex];
            if (slot.Pokemon.Species != 0) // Ensure it's not empty
            {
                await Navigation.PushAsync(new PokemonEditorPage(slot.Pokemon));
            }
        }
    }
}
