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
    public partial class SaveEditorControl : UserControl
    {
        private readonly SaveContext _context;
        private IPokemon? _currentPokemon;
        private IPokemon? _originalPokemonState;
        private int _currentSlot = -1;

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
            
            var displayList = boxPokemon.Select((p, i) => new { Index = i, Pokemon = p, Display = p.Species == 0 ? $"[{i + 1}] (Empty)" : $"[{i + 1}] {p.Nickname} (Species {p.Species})" }).ToList();
            
            BoxList.ItemsSource = displayList;
            BoxList.DisplayMemberPath = "Display";
        }

        private void BoxList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BoxList.SelectedItem == null) return;

            dynamic item = BoxList.SelectedItem;
            _currentPokemon = item.Pokemon as IPokemon;
            _originalPokemonState = _currentPokemon?.Clone();
            _currentSlot = item.Index;

            if (_currentPokemon != null && _currentPokemon.Species != 0)
            {
                SpeciesTextBox.Text = _currentPokemon.Species.ToString();
                NicknameTextBox.Text = _currentPokemon.Nickname;
                LevelTextBox.Text = _currentPokemon.Level.ToString();
                
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
            LegalityText.Text = "No Pokémon Selected";
            LegalityBadge.Background = (SolidColorBrush)FindResource("BackgroundTertiary");
            LegalityReportBlock.Text = "Load a Pokémon to see legality.";
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

            var command = new RotoDex.Core.Commands.EditPokemonCommand(_context.SaveFile, BoxSelector.SelectedIndex, _currentSlot, _originalPokemonState, newState);
            _context.CommandManager.Execute(command);
            
            _originalPokemonState = newState.Clone();
            
            RunLegalityCheck(newState);

            int selectedIndex = BoxSelector.SelectedIndex;
            int listIndex = BoxList.SelectedIndex;
            BoxSelector_SelectionChanged(null, null);
            BoxList.SelectedIndex = listIndex;
        }

        public void RefreshData()
        {
            int boxIdx = BoxSelector.SelectedIndex;
            int listIdx = BoxList.SelectedIndex;
            if (boxIdx >= 0)
            {
                BoxSelector_SelectionChanged(null, null);
                if (listIdx >= 0 && listIdx < BoxList.Items.Count)
                    BoxList.SelectedIndex = listIdx;
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
