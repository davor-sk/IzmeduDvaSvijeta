using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class SaveData{
    public string nodeName = "";

    
    public List<string> floatKeys = new List<string>();
    public List<float> floatValues = new List<float>();
    public List<string> stringKeys = new List<string>();
    public List<string> stringValues = new List<string>();
    public List<string> boolKeys = new List<string>();
    public List<bool> boolValues = new List<bool>();

  
    public List<string> notebookWords = new List<string>();
    public List<string> notebookTranslations = new List<string>();
}


public static class SaveSystem
{
    private const string FileName = "savegame.json";


    public static bool ContinueRequested = false;

    private static string Path =>
        System.IO.Path.Combine(Application.persistentDataPath, FileName);

    public static bool HasSave()
    {
        return File.Exists(Path);
    }

    public static void Save(SaveData data)
    {
        try
        {
            File.WriteAllText(Path, JsonUtility.ToJson(data, true));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Spremanje nije uspjelo: " + e.Message);
        }
    }

    public static SaveData Load()
    {
        if (!HasSave()) return null;

        try
        {
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(Path));

         
            if (data == null || string.IsNullOrEmpty(data.nodeName))
            {
                return null;
            }

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Učitavanje nije uspjelo: " + e.Message);
            return null;
        }
    }

    public static void Delete()
    {
        try
        {
            if (File.Exists(Path)) File.Delete(Path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Brisanje spremljene igre nije uspjelo: " + e.Message);
        }
    }
}
