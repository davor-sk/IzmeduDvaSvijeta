using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Trajni napredak otkrica (clueova), odvojeno od savegame.json.
// Zbog research izbora nije moguce skupiti svih 33 u jednom prolazu,
// pa se napredak zbraja kroz vise playthroughova.
[System.Serializable]
public class ClueProgressData
{
    public List<string> unlockedClues = new List<string>();
}

public static class ClueSaveSystem
{
    private const string FileName = "clues.json";

    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static void Save(ClueProgressData data)
    {
        try
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(data, true));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Spremanje otkrica nije uspjelo: " + e.Message);
        }
    }

    public static ClueProgressData Load()
    {
        if (!File.Exists(FilePath))
            return new ClueProgressData();

        try
        {
            var data = JsonUtility.FromJson<ClueProgressData>(File.ReadAllText(FilePath));

            if (data == null)
                return new ClueProgressData();

            if (data.unlockedClues == null)
                data.unlockedClues = new List<string>();

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ucitavanje otkrica nije uspjelo: " + e.Message);
            return new ClueProgressData();
        }
    }

    public static void ResetProgress()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
            Debug.Log("Otkrica obrisana.");
        }
    }
}
