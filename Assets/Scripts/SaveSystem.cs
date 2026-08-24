using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class NotebookWordEntry
{
    public string dhornWord;
    public string translation;
}

[System.Serializable]
public class SaveData
{
    public string currentNode;

    
    public List<string> floatKeys = new List<string>();
    public List<float> floatValues = new List<float>();

    public List<string> stringKeys = new List<string>();
    public List<string> stringValues = new List<string>();

    public List<string> boolKeys = new List<string>();
    public List<bool> boolValues = new List<bool>();

   
    public List<NotebookWordEntry> notebookWords = new List<NotebookWordEntry>();

    public string savedAtDisplay;
}

public static class SaveSystem
{
    private const string FileName = "savegame.json";

    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static bool HasSave()
    {
        return File.Exists(FilePath);
    }

    public static void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
            Debug.Log("Igra spremljena: " + FilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Spremanje nije uspjelo: " + e.Message);
        }
    }

    public static SaveData Load()
    {
        if (!HasSave())
            return null;

        try
        {
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Učitavanje nije uspjelo: " + e.Message);
            return null;
        }
    }

    public static void DeleteSave()
    {
        if (HasSave())
        {
            File.Delete(FilePath);
            Debug.Log("Spremljena igra obrisana.");
        }
    }
}
