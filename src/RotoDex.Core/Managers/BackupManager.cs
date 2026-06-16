using System;
using System.IO;

namespace RotoDex.Core.Managers;

public class BackupManager
{
    public string BackupDirectory { get; }

    public BackupManager(string backupDirectory = "backups")
    {
        BackupDirectory = backupDirectory;
        if (!Directory.Exists(BackupDirectory))
        {
            Directory.CreateDirectory(BackupDirectory);
        }
    }

    public void BackupSave(string originalFilePath)
    {
        if (!File.Exists(originalFilePath))
            return;

        string fileName = Path.GetFileNameWithoutExtension(originalFilePath);
        string extension = Path.GetExtension(originalFilePath);
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        
        string backupFileName = $"{fileName}_{timestamp}{extension}.bak";
        string backupPath = Path.Combine(BackupDirectory, backupFileName);

        File.Copy(originalFilePath, backupPath, overwrite: true);
    }
}
