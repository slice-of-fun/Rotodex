using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using RotoDex.Core.Managers;
using RotoDex.Core.Plugins;
using System.IO;

namespace RotoDex.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly SaveManager _saveManager;
        private readonly PluginLoader _pluginLoader;

        public MainWindow(SaveManager saveManager, PluginLoader pluginLoader)
        {
            InitializeComponent();
            _saveManager = saveManager;
            _saveManager.SaveOpened += SaveManager_SaveOpened;
            _saveManager.SaveClosed += SaveManager_SaveClosed;
            _pluginLoader = pluginLoader;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Save Files (*.sav;*.main)|*.sav;*.main|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _saveManager.OpenFile(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading save: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveActiveFile_Click(object sender, RoutedEventArgs e)
        {
            if (_saveManager.ActiveContext == null) return;

            try
            {
                _saveManager.SaveActiveFile();
                MessageBox.Show("Active save successfully written!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseActiveFile_Click(object sender, RoutedEventArgs e)
        {
            if (_saveManager.ActiveContext != null)
            {
                _saveManager.CloseFile(_saveManager.ActiveContext);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (SaveTabs.SelectedItem is TabItem activeTab && activeTab.Content is SaveEditorControl editor)
            {
                editor.Undo();
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (SaveTabs.SelectedItem is TabItem activeTab && activeTab.Content is SaveEditorControl editor)
            {
                editor.Redo();
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                if (e.Key == System.Windows.Input.Key.Z)
                {
                    Undo_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                }
                else if (e.Key == System.Windows.Input.Key.Y)
                {
                    Redo_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                }
            }
        }

        private void OpenBatchEditor_Click(object sender, RoutedEventArgs e)
        {
            if (_saveManager.ActiveContext == null)
            {
                MessageBox.Show("Please open a save file first.", "No Save Loaded", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var activeTab = SaveTabs.SelectedItem as TabItem;
            if (activeTab?.Content is SaveEditorControl editorControl)
            {
                var batchEditor = new BatchEditorWindow(_saveManager.ActiveContext, editorControl.GetActiveBoxIndex());
                batchEditor.Owner = this;
                batchEditor.ShowDialog();
                
                editorControl.RefreshData();
            }
        }

        private void SaveManager_SaveOpened(object? sender, SaveContext context)
        {
            var editorControl = new SaveEditorControl(context);
            
            var tabItem = new TabItem
            {
                Header = context.DisplayName,
                Content = editorControl,
                Tag = context
            };
            
            SaveTabs.Items.Add(tabItem);
            SaveTabs.SelectedItem = tabItem;

            UpdateWindowStates();
        }

        private void SaveManager_SaveClosed(object? sender, SaveContext context)
        {
            var tabToRemove = SaveTabs.Items.Cast<TabItem>().FirstOrDefault(t => t.Tag == context);
            if (tabToRemove != null)
            {
                SaveTabs.Items.Remove(tabToRemove);
            }

            UpdateWindowStates();
        }

        private void UpdateWindowStates()
        {
            if (SaveTabs.Items.Count > 0)
            {
                WelcomePanel.Visibility = Visibility.Collapsed;
                SaveTabs.Visibility = Visibility.Visible;
            }
            else
            {
                WelcomePanel.Visibility = Visibility.Visible;
                SaveTabs.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl && SaveTabs.SelectedItem is TabItem selectedTab)
            {
                _saveManager.ActiveContext = selectedTab.Tag as SaveContext;
            }
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string filePath = files[0];
                    try
                    {
                        _saveManager.OpenFile(filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading save: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}