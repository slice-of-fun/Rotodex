using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RotoDex.Adapter;
using RotoDex.Analyzer;
using RotoDex.Core.Managers;

namespace RotoDex.Desktop
{
    public class BoxSlotViewModel
    {
        public int Index { get; set; }
        public IPokemon Pokemon { get; set; } = null!;
        public string DisplayName { get; set; } = "";
        public string SpriteUrl { get; set; } = "";
        public bool IsSelected { get; set; }
        public bool IsEmpty => Pokemon == null || Pokemon.Species == 0;
    }

    public partial class SaveEditorControl : UserControl
    {
        private readonly SaveContext _context;
        private IPokemon? _currentPokemon;
        private IPokemon? _originalPokemonState;
        private int _currentSlot = -1;
        private List<BoxSlotViewModel> _currentBoxSlots = new List<BoxSlotViewModel>();

        public SaveEditorControl(SaveContext context)
        {
            InitializeComponent();
            _context = context;
            LoadSaveData();
        }

        private void LoadSaveData()
        {
            if (_context.SaveFile == null) return;

            BoxSelector.Items.Clear();
            for (int i = 0; i < _context.SaveFile.BoxCount; i++)
            {
                BoxSelector.Items.Add($"Box {i + 1}");
            }

            if (BoxSelector.Items.Count > 0)
                BoxSelector.SelectedIndex = 0;
        }

        private void BoxSelector_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
            if (_context.SaveFile == null || BoxSelector.SelectedIndex < 0) return;

            var boxPokemon = _context.SaveFile.GetBox(BoxSelector.SelectedIndex).ToList();
            
            _currentBoxSlots = boxPokemon.Select((p, i) => new BoxSlotViewModel
            {
                Index = i,
                Pokemon = p,
                DisplayName = p.Species == 0 ? $"" : $"{p.Nickname}\nLvl {p.Level}",
                SpriteUrl = p.Species == 0 ? "" : $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{p.Species}.png",
                IsSelected = (i == _currentSlot)
            }).ToList();
            
            BoxGrid.ItemsSource = _currentBoxSlots;
        }

        private void SlotButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int index)
            {
                SelectSlot(index);
            }
        }

        private void SelectSlot(int index)
        {
            if (index < 0 || index >= _currentBoxSlots.Count) return;

            _currentSlot = index;

            // Update IsSelected flags
            foreach (var slot in _currentBoxSlots)
            {
                slot.IsSelected = (slot.Index == index);
            }

            // Refresh storage grid items binding
            BoxGrid.ItemsSource = null;
            BoxGrid.ItemsSource = _currentBoxSlots;

            var selectedSlot = _currentBoxSlots[index];
            _currentPokemon = selectedSlot.Pokemon;
            _originalPokemonState = _currentPokemon?.Clone();

            if (_currentPokemon != null && _currentPokemon.Species != 0)
            {
                // Load Main data
                SpeciesTextBox.Text = _currentPokemon.Species.ToString();
                NicknameTextBox.Text = _currentPokemon.Nickname;
                LevelTextBox.Text = _currentPokemon.Level.ToString();
                NatureTextBox.Text = _currentPokemon.Nature.ToString();
                GenderTextBox.Text = _currentPokemon.Gender.ToString();
                
                // Load IVs
                var ivs = _currentPokemon.IVs;
                IvHpTextBox.Text = ivs.Length > 0 ? ivs[0].ToString() : "0";
                IvAtkTextBox.Text = ivs.Length > 1 ? ivs[1].ToString() : "0";
                IvDefTextBox.Text = ivs.Length > 2 ? ivs[2].ToString() : "0";
                IvSpaTextBox.Text = ivs.Length > 3 ? ivs[3].ToString() : "0";
                IvSpdTextBox.Text = ivs.Length > 4 ? ivs[4].ToString() : "0";
                IvSpeTextBox.Text = ivs.Length > 5 ? ivs[5].ToString() : "0";

                // Load EVs
                var evs = _currentPokemon.EVs;
                EvHpTextBox.Text = evs.Length > 0 ? evs[0].ToString() : "0";
                EvAtkTextBox.Text = evs.Length > 1 ? evs[1].ToString() : "0";
                EvDefTextBox.Text = evs.Length > 2 ? evs[2].ToString() : "0";
                EvSpaTextBox.Text = evs.Length > 3 ? evs[3].ToString() : "0";
                EvSpdTextBox.Text = evs.Length > 4 ? evs[4].ToString() : "0";
                EvSpeTextBox.Text = evs.Length > 5 ? evs[5].ToString() : "0";

                // Load Moves
                var moves = _currentPokemon.Moves;
                Move1TextBox.Text = moves.Length > 0 ? moves[0].ToString() : "0";
                Move2TextBox.Text = moves.Length > 1 ? moves[1].ToString() : "0";
                Move3TextBox.Text = moves.Length > 2 ? moves[2].ToString() : "0";
                Move4TextBox.Text = moves.Length > 3 ? moves[3].ToString() : "0";

                // Load sprite icon
                if (!string.IsNullOrEmpty(selectedSlot.SpriteUrl))
                {
                    try
                    {
                        PokemonDetailSprite.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(selectedSlot.SpriteUrl));
                        PokemonDetailSpriteContainer.Visibility = Visibility.Visible;
                        PokemonDetailSpritePlaceholder.Visibility = Visibility.Collapsed;
                    }
                    catch
                    {
                        PokemonDetailSpriteContainer.Visibility = Visibility.Collapsed;
                        PokemonDetailSpritePlaceholder.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    PokemonDetailSpriteContainer.Visibility = Visibility.Collapsed;
                    PokemonDetailSpritePlaceholder.Visibility = Visibility.Visible;
                }

                RunLegalityCheck(_currentPokemon);
            }
            else
            {
                ClearEditor();
            }
        }

        private void ClearEditor()
        {
            SpeciesTextBox.Text = "";
            NicknameTextBox.Text = "";
            LevelTextBox.Text = "";
            NatureTextBox.Text = "";
            GenderTextBox.Text = "";
            
            IvHpTextBox.Text = "";
            IvAtkTextBox.Text = "";
            IvDefTextBox.Text = "";
            IvSpaTextBox.Text = "";
            IvSpdTextBox.Text = "";
            IvSpeTextBox.Text = "";
            
            EvHpTextBox.Text = "";
            EvAtkTextBox.Text = "";
            EvDefTextBox.Text = "";
            EvSpaTextBox.Text = "";
            EvSpdTextBox.Text = "";
            EvSpeTextBox.Text = "";
            
            Move1TextBox.Text = "";
            Move2TextBox.Text = "";
            Move3TextBox.Text = "";
            Move4TextBox.Text = "";

            LegalityText.Text = "No Pokémon Selected";
            LegalityBadge.Background = (SolidColorBrush)FindResource("BackgroundTertiary");
            LegalityReportBlock.Text = "Load a Pokémon to see legality.";
            
            PokemonDetailSpriteContainer.Visibility = Visibility.Collapsed;
            PokemonDetailSpritePlaceholder.Visibility = Visibility.Visible;
        }

        private void RunLegalityCheck(IPokemon pokemon)
        {
            try
            {
                var report = LegalityWrapper.CheckLegality(pokemon);
                
                if (report.IsLegal)
                {
                    LegalityText.Text = "Valid";
                    LegalityBadge.Background = (SolidColorBrush)FindResource("SuccessBrush");
                }
                else
                {
                    LegalityText.Text = "Invalid";
                    LegalityBadge.Background = (SolidColorBrush)FindResource("ErrorBrush");
                }

                var sb = new StringBuilder();
                AppendList(sb, "Origin", report.OriginChecks);
                AppendList(sb, "Encounter", report.EncounterChecks);
                AppendList(sb, "Ball", report.BallChecks);
                AppendList(sb, "Moves", report.MoveChecks);
                AppendList(sb, "Stats", report.StatsChecks);
                AppendList(sb, "Ribbons", report.RibbonChecks);
                AppendList(sb, "PID & Encryption", report.PIDChecks);
                AppendList(sb, "Generation Path", report.PathChecks);
                AppendList(sb, "Miscellaneous", report.MiscChecks);

                if (sb.Length == 0)
                {
                    sb.AppendLine("No legality warnings generated.");
                }

                LegalityReportBlock.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                LegalityText.Text = "Error";
                LegalityBadge.Background = (SolidColorBrush)FindResource("ErrorBrush");
                LegalityReportBlock.Text = $"Analyzer failed: {ex.Message}";
            }
        }

        private void AppendList(StringBuilder sb, string category, System.Collections.Generic.List<string> items)
        {
            if (items != null && items.Count > 0)
            {
                sb.AppendLine($"--- {category} ---");
                foreach(var item in items)
                {
                    sb.AppendLine($"• {item}");
                }
                sb.AppendLine();
            }
        }

        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            if (_originalPokemonState == null || _context.SaveFile == null || BoxSelector.SelectedIndex < 0 || _currentSlot < 0) return;

            var newState = _originalPokemonState.Clone();

            if (ushort.TryParse(SpeciesTextBox.Text, out ushort species))
                newState.Species = species;

            if (byte.TryParse(LevelTextBox.Text, out byte level))
                newState.Level = level;

            newState.Nickname = NicknameTextBox.Text;

            if (int.TryParse(NatureTextBox.Text, out int nature))
                newState.Nature = nature;

            if (int.TryParse(GenderTextBox.Text, out int gender))
                newState.Gender = gender;

            // Apply IVs
            int[] ivs = new int[6];
            int.TryParse(IvHpTextBox.Text, out ivs[0]);
            int.TryParse(IvAtkTextBox.Text, out ivs[1]);
            int.TryParse(IvDefTextBox.Text, out ivs[2]);
            int.TryParse(IvSpaTextBox.Text, out ivs[3]);
            int.TryParse(IvSpdTextBox.Text, out ivs[4]);
            int.TryParse(IvSpeTextBox.Text, out ivs[5]);
            newState.IVs = ivs;

            // Apply EVs
            byte[] evs = new byte[6];
            byte.TryParse(EvHpTextBox.Text, out evs[0]);
            byte.TryParse(EvAtkTextBox.Text, out evs[1]);
            byte.TryParse(EvDefTextBox.Text, out evs[2]);
            byte.TryParse(EvSpaTextBox.Text, out evs[3]);
            byte.TryParse(EvSpdTextBox.Text, out evs[4]);
            byte.TryParse(EvSpeTextBox.Text, out evs[5]);
            newState.EVs = evs;

            // Apply Moves
            ushort[] moves = new ushort[4];
            ushort.TryParse(Move1TextBox.Text, out moves[0]);
            ushort.TryParse(Move2TextBox.Text, out moves[1]);
            ushort.TryParse(Move3TextBox.Text, out moves[2]);
            ushort.TryParse(Move4TextBox.Text, out moves[3]);
            newState.Moves = moves;

            var command = new RotoDex.Core.Commands.EditPokemonCommand(_context.SaveFile, BoxSelector.SelectedIndex, _currentSlot, _originalPokemonState, newState);
            _context.CommandManager.Execute(command);
            
            _originalPokemonState = newState.Clone();
            
            RunLegalityCheck(newState);

            // Refresh box storage grid to show updated sprite / level
            int currentSlotIdx = _currentSlot;
            BoxSelector_SelectionChanged(null, null);
            SelectSlot(currentSlotIdx);
        }

        public void RefreshData()
        {
            int boxIdx = BoxSelector.SelectedIndex;
            int slotIdx = _currentSlot;
            if (boxIdx >= 0)
            {
                BoxSelector_SelectionChanged(null, null);
                if (slotIdx >= 0 && slotIdx < _currentBoxSlots.Count)
                    SelectSlot(slotIdx);
            }
        }
        
        public int GetActiveBoxIndex()
        {
            return BoxSelector.SelectedIndex >= 0 ? BoxSelector.SelectedIndex : 0;
        }

        public void Undo()
        {
            _context.CommandManager.Undo();
            RefreshData();
        }

        public void Redo()
        {
            _context.CommandManager.Redo();
            RefreshData();
        }
    }
}
