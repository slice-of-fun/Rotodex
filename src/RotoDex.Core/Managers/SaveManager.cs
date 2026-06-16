using System;
using System.IO;
using System.Collections.Generic;
using RotoDex.Adapter;

namespace RotoDex.Core.Managers;

public class SaveManager
{
    private readonly BackupManager _backupManager;
    private readonly List<SaveContext> _openSaves = new List<SaveContext>();
    
    public IReadOnlyList<SaveContext> OpenSaves => _openSaves;
    public SaveContext? ActiveContext { get; set; }

    // Legacy properties for quick compat if needed, but ideally we use ActiveContext
    public ISaveFile? CurrentSave => ActiveContext?.SaveFile;
    public string? CurrentFilePath => ActiveContext?.FilePath;

    public event EventHandler<SaveContext>? SaveOpened;
    public event EventHandler<SaveContext>? SaveClosed;

    public SaveManager(BackupManager backupManager)
    {
        _backupManager = backupManager ?? throw new ArgumentNullException(nameof(backupManager));
    }

    public SaveContext OpenFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Save file not found.", path);
            
        // Trigger auto-backup
        _backupManager.BackupSave(path);
        
        byte[] data = File.ReadAllBytes(path);
        var saveAdapter = new SaveAdapter(data);
        var context = new SaveContext(saveAdapter, path);
        
        _openSaves.Add(context);
        ActiveContext = context;
        
        SaveOpened?.Invoke(this, context);
        return context;
    }

    public void SaveFile(SaveContext context, string? newPath = null)
    {
        if (context == null || context.SaveFile == null)
            throw new InvalidOperationException("Invalid save context.");

        string targetPath = newPath ?? context.FilePath;
        byte[] data = context.SaveFile.Write();
        File.WriteAllBytes(targetPath, data);
    }
    
    public void SaveActiveFile()
    {
        if (ActiveContext != null)
            SaveFile(ActiveContext);
    }

    public void CloseFile(SaveContext context)
    {
        if (_openSaves.Remove(context))
        {
            if (ActiveContext == context)
            {
                ActiveContext = _openSaves.Count > 0 ? _openSaves[_openSaves.Count - 1] : null;
            }
            SaveClosed?.Invoke(this, context);
        }
    }
}
