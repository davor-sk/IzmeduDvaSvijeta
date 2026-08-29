using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

// Trajno pamti koje je krajeve igrac vidio, kroz vise playthroughova.
public static class EndingManager
{
    public const int TotalEndings = 4;

    // Kanonski redoslijed za prikaz u izborniku
    public static readonly string[] AllEndingIds =
    {
        "dhorn",
        "government",
        "truth",
        "war"
    };

    private static readonly Dictionary<string, string> DisplayNames =
        new Dictionary<string, string>
        {
            { "dhorn",      "Dhorn" },
            { "government", "Vlada" },
            { "truth",      "Istina" },
            { "war",        "Rat" }
        };

    private static EndingProgressData cached;

    private static EndingProgressData Data
    {
        get
        {
            if (cached == null)
                cached = EndingSaveSystem.Load();

            return cached;
        }
    }

    // glavna funkcija koja oznacava kraj kao unlocked i trajno spremma

    public static void AddEnding(string endingId)
    {
        if (string.IsNullOrEmpty(endingId))
        {
            Debug.LogWarning("AddEnding: prazan id kraja.");
            return;
        }

        endingId = endingId.Trim().ToLowerInvariant();

        if (!DisplayNames.ContainsKey(endingId))
        {
            Debug.LogWarning("AddEnding: nepoznat id kraja: " + endingId);
            return;
        }

        if (Data.unlockedEndings.Contains(endingId))
        {
            Debug.Log("Kraj vec otkljucan: " + endingId);
            return;
        }

        Data.unlockedEndings.Add(endingId);
        EndingSaveSystem.Save(Data);

        Debug.Log("Novi kraj otkljucan: " + endingId +
                  " (" + UnlockedCount + "/" + TotalEndings + ")");

        // Cetvrti kraj otkljucava achievement za sve zavrsetke
        AchievementManager.CheckAllEndings();
    }

    [YarnCommand("unlock_ending")]
    public static void UnlockEndingFromYarn(string endingId)
    {
        AddEnding(endingId);
    }

    public static bool IsUnlocked(string endingId)
    {
        if (string.IsNullOrEmpty(endingId))
            return false;

        return Data.unlockedEndings.Contains(endingId.Trim().ToLowerInvariant());
    }

    public static int UnlockedCount => Data.unlockedEndings.Count;

    public static bool AllEndingsUnlocked()
    {
        return UnlockedCount >= TotalEndings;
    }

    public static string GetDisplayName(string endingId)
    {
        if (!string.IsNullOrEmpty(endingId) &&
            DisplayNames.TryGetValue(endingId.Trim().ToLowerInvariant(), out string name))
        {
            return name;
        }

        return endingId;
    }

    public static string GetProgressText()
    {
        return UnlockedCount + "/" + TotalEndings;
    }

    public static void ResetAll()
    {
        cached = new EndingProgressData();
        EndingSaveSystem.ResetProgress();
    }
}
