using System;
using System.IO;
using UnityEngine;

public class SaveFileToDownload : MonoBehaviour
{
    public void SaveFile(string fileName, byte[] fileData)
    {
        string downloadsPath = GetDownloadsPath();
        if (string.IsNullOrEmpty(downloadsPath))
        {
            Debug.LogError("Could not find the Downloads folder.");
            return;
        }

        string fullPath = Path.Combine(downloadsPath, fileName);

        try
        {
            File.WriteAllBytes(fullPath, fileData);
            Debug.Log("File saved successfully: " + fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save file: " + e.Message);
        }
    }

    private string GetDownloadsPath()
    {
        string path = string.Empty;

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Downloads");
        }
        else if (Application.platform == RuntimePlatform.LinuxPlayer)
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Downloads");
        }

        return path;
    }
}
