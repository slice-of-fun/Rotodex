using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Roto.Core;

#if !DEBUG
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#endif

namespace RotoDex.Desktop;

public static class Program
{
    // Pipelines build can sometimes tack on text to the version code. Strip it out.
    public static readonly Version CurrentVersion = Version.Parse(GetSaneVersionTag(Application.ProductVersion));

    public static readonly string WorkingDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? "";
    public const string ConfigFileName = "cfg.json";
    public static string PathConfig => Path.Combine(WorkingDirectory, ConfigFileName);

    /// <summary>
    /// Global settings instance, loaded before any forms are created.
    /// </summary>
    public static RotoDexSettings Settings { get; }

    public static bool HaX { get; private set; }
    static Program()
    {
#if !DEBUG
        Application.ThreadException += UIThreadException;
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Settings = RotoDexSettings.GetSettings(PathConfig);

        if (Settings.Startup.DarkMode)
            Application.SetColorMode(SystemColorMode.Dark);
        if (Settings.Startup.HighDpiText)
            Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
    }

    [STAThread]
    private static void Main()
    {
        // Load settings first
        var settings = Settings;
        settings.LocalResources.SetLocalPath(WorkingDirectory);
        StartupUtil.ReloadSettings(settings);

        SplashScreen? splash = null;
        if (!settings.Startup.SkipSplashScreen)
        {
            // Show splash screen on a dedicated STA thread so it can pump its own message loop.
            var splashThread = new Thread(() =>
            {
                splash = new SplashScreen();
                Application.Run(splash);
            })
            { IsBackground = true };
            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start();
        }

        var args = Environment.GetCommandLineArgs();
        // Prepare initial values for the main form.
        var startup = StartupUtil.GetStartup(args, settings);
        var init = StartupUtil.FormLoadInitialActions(args, settings, CurrentVersion);
        HaX = init.HaX;
        var main = new Main();

        // Close splash when Main is ready to display, then perform startup animation.
        main.Shown += async (_, _) =>
        {
            splash?.BeginInvoke(splash.ForceClose);
            main.Activate();

            // Follow-up: display popups if needed.
            if (init.HaX)
                main.WarnBehavior();
            else if (init.ShowChangelog)
                main.ShowAboutDialog(AboutPage.Changelog);
            else if (init.BackupPrompt)
                main.PromptBackup(settings.LocalResources.GetBackupPath());

            await main.CheckForUpdates().ConfigureAwait(true);
        };

        // Setup complete.
        if (Settings.Startup.PluginLoadEnable)
            main.AttachPlugins();
        main.LoadInitialFiles(startup);
        Application.Run(main);
    }

    private static ReadOnlySpan<char> GetSaneVersionTag(ReadOnlySpan<char> productVersion)
    {
        for (int i = 0; i < productVersion.Length; i++)
        {
            char c = productVersion[i];
            if (c == '.')
                continue;
            if (char.IsNumber(c))
                continue;
            return productVersion[..i];
        }
        return productVersion;
    }

#if !DEBUG
    private static void Error(string msg) => MessageBox.Show(msg, "RotoDex Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);

    private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
    {
        DialogResult result = DialogResult.Cancel;
        try
        {
            var e = t.Exception;
            string errorMessage = GetErrorMessage(e);
            result = ErrorWindow.ShowErrorDialog(errorMessage, e, true);
        }
        catch (Exception reportingException)
        {
            HandleReportingException(t.Exception, reportingException);
        }

        if (result == DialogResult.Abort)
            Application.Exit();
    }

    private static string GetErrorMessage(Exception e)
    {
        try
        {
            if (IsPluginError<IPlugin>(e, out var pluginName))
                return $"An error occurred in a RotoDex plugin. Please report this error to the plugin author/maintainer.\n{pluginName}";
        }
        catch
        {
            // If we fail to analyze the stack trace, just return the generic message. Don't risk another exception.
        }
        return "An error occurred in RotoDex. Please report this error to the RotoDex author.";
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        try
        {
            if (IsOldPkhexCorePresent(ex))
            {
                Error("You have upgraded RotoDex incorrectly. Please delete Roto.Core.dll.");
            }
            else if (IsPkhexCoreMissing(ex))
            {
                Error("You have installed RotoDex incorrectly. Please ensure you have unzipped all files before running.");
            }
            else if (ex is not null)
            {
                var msg = GetErrorMessage(ex);
                ErrorWindow.ShowErrorDialog($"{msg}\nRotoDex must now close.", ex, false);
            }
            else
            {
                Error("A fatal non-UI error has occurred in RotoDex, and the details could not be displayed.  Please report this to the author.");
            }
        }
        catch (Exception reportingException)
        {
            HandleReportingException(ex, reportingException);
        }
    }

    private static bool IsPluginError<T>(Exception exception, out string pluginName)
    {
        pluginName = string.Empty;
        var stackTrace = new System.Diagnostics.StackTrace(exception);
        foreach (var frame in stackTrace.GetFrames())
        {
            var method = frame.GetMethod();
            var type = method?.DeclaringType;
            if (!typeof(T).IsAssignableFrom(type))
                continue;
            pluginName = type.Namespace ?? string.Empty;
            return true;
        }
        return false;
    }

    private static void HandleReportingException(Exception? ex, Exception reportingException)
    {
        try
        {
            EmergencyErrorLog(ex, reportingException);
        }
        catch
        {
            // Do nothing. If we can't log the error, there's not much else we can do, and we don't want to risk another exception.
        }
        if (reportingException is FileNotFoundException x && x.FileName?.StartsWith("Roto.Core") == true)
        {
            Error("Could not locate Roto.Core.dll. Make sure you're running RotoDex together with its code library. Usually caused when all files are not extracted.");
            return;
        }
        try
        {
            Error("A fatal non-UI error has occurred in RotoDex, and there was a problem displaying the details.  Please report this to the author.");
        }
        finally
        {
            Application.Exit();
        }
    }

    private static bool EmergencyErrorLog(Exception? originalException, Exception errorHandlingException)
    {
        try
        {
            var message = (originalException?.ToString() ?? "null first exception") + Environment.NewLine + errorHandlingException;
            File.WriteAllText($"RotoDex_Error_Report {DateTime.Now:yyyyMMddHHmmss}.txt", message);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    private static bool IsOldPkhexCorePresent([NotNullWhen(true)] Exception? ex)
    {
        return ex is MissingMethodException or TypeLoadException or TypeInitializationException
            && File.Exists("Roto.Core.dll")
            && AssemblyName.GetAssemblyName("Roto.Core.dll").Version < CurrentVersion;
    }

    private static bool IsPkhexCoreMissing([NotNullWhen(true)] Exception? ex)
    {
        return ex is FileNotFoundException { FileName: {} n } && (n.Contains("Roto.Core") || n.Contains("RotoDex.Core"));
    }
#endif
}
