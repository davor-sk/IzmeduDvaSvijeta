using System.Collections.Generic;
using System.IO;
using UnityEngine;


// podaci se ne brisu kad se ponovno pokrene igra
[System.Serializable]
public class EndingProgressData
{
    // Id-evi otkljucanih krajeva: "dhorn", "government", "truth", "war"
    public List<string> unlockedEndings = new List<string>();
}

public static class EndingSaveSystem
{
    private const string FileName = "endings.json";

    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static bool HasProgress()
    {
        return File.Exists(FilePath);
    }

    public static void Save(EndingProgressData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
            Debug.Log("Napredak krajeva spremljen: " + FilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Spremanje krajeva nije uspjelo: " + e.Message);
        }
    }

    public static EndingProgressData Load()
    {
        if (!HasProgress())
            return new EndingProgressData();

        try
        {
            string json = File.ReadAllText(FilePath);
            var data = JsonUtility.FromJson<EndingProgressData>(json);

            
            if (data == null)
                return new EndingProgressData();

            if (data.unlockedEndings == null)
                data.unlockedEndings = new List<string>();

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ucitavanje krajeva nije uspjelo: " + e.Message);
            return new EndingProgressData();
        }
    }

    public static void ResetProgress()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
            Debug.Log("Napredak krajeva obrisan.");
        }
    }
}
