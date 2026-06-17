using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using RotoDex.Core.Managers;
using RotoDex.Core.Plugins;

namespace RotoDex.Desktop
{
    public partial class SplashWindow : Window
    {
        private SaveManager? _saveManager;
        private PluginLoader? _pluginLoader;

        public SplashWindow()
        {
            InitializeComponent();
            Loaded += SplashWindow_Loaded;
        }

        private async void SplashWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Step 1: Initialize System Cores
                UpdateStatus("Initializing database engines...", 20);
                await Task.Delay(400);

                // Step 2: Initialize Save Manager and Backup systems
                UpdateStatus("Loading file adapters & backups...", 50);
                _saveManager = new SaveManager(new BackupManager());
                await Task.Delay(400);

                // Step 3: Scan and Load Plugins
                UpdateStatus("Scanning plugin modules...", 80);
                var modContext = new ModContext(_saveManager);
                _pluginLoader = new PluginLoader();
                string rotomodsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rotomods");
                
                // Create directory if not exists
                if (!Directory.Exists(rotomodsDir))
                {
                    Directory.CreateDirectory(rotomodsDir);
                }

                // Load plugins
                await Task.Run(() => _pluginLoader.LoadPlugins(rotomodsDir, modContext));
                await Task.Delay(400);

                // Step 4: Finished
                UpdateStatus("System ready!", 100);
                await Task.Delay(300);

                // Open MainWindow and close this splash screen
                var mainWindow = new MainWindow(_saveManager, _pluginLoader);
                App.Current.MainWindow = mainWindow;
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup failed: {ex.Message}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void UpdateStatus(string status, double progress)
        {
            StatusText.Text = status;
            ProgressBar.Value = progress;
        }
    }
}
