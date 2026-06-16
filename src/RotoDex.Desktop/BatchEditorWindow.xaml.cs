using System;
using System.Linq;
using System.Windows;
using RotoDex.Core.Managers;
using RotoDex.Adapter;

namespace RotoDex.Desktop
{
    public partial class BatchEditorWindow : Window
    {
        private readonly SaveContext _context;
        private readonly int _currentBoxIndex;

        public BatchEditorWindow(SaveContext context, int currentBoxIndex)
        {
            InitializeComponent();
            _context = context;
            _currentBoxIndex = currentBoxIndex;
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            if (_context == null || _context.SaveFile == null) return;

            var lines = ScriptInput.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) return;

            var targetBoxes = BoxTargetComboBox.SelectedIndex == 0 
                ? Enumerable.Range(0, _context.SaveFile.BoxCount) 
                : new[] { _currentBoxIndex };

            var batchManager = new BatchExecutionManager();
            
            try
            {
                int modified = batchManager.ExecuteBatch(_context.SaveFile, lines, targetBoxes);
                MessageBox.Show($"Batch execution complete!\n{modified} Pokémon were modified.", "Batch Result", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Batch execution failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
