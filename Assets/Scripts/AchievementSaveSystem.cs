using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Trajni napredak achievementa, odvojeno od savegame.json.
[System.Serializable]
public class AchievementProgressData
{
    public List<string> unlockedAchievements = new List<string>();
}

public static class AchievementSaveSystem
{
    private const string FileName = "achievements.json";

    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static void Save(AchievementProgressData data)
    {
        try
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(data, true));
            Debug.Log("Achievementi spremljeni: " + FilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Spremanje achievementa nije uspjelo: " + e.Message);
        }
    }

    public static AchievementProgressData Load()
    {
        if (!File.Exists(FilePath))
            return new AchievementProgressData();

        try
        {
            var data = JsonUtility.FromJson<AchievementProgressData>(File.ReadAllText(FilePath));

            if (data == null)
                return new AchievementProgressData();

            if (data.unlockedAchievements == null)
                data.unlockedAchievements = new List<string>();

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ucitavanje achievementa nije uspjelo: " + e.Message);
            return new AchievementProgressData();
        }
    }

    public static void ResetProgress()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
            Debug.Log("Achievementi obrisani.");
        }
    }
}
