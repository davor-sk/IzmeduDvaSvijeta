using System;
using System.Collections.Generic;
using UnityEngine;

public static class AchievementManager
{
    public const string Polyglot   = "POLYGLOT";
    public const string Investigator = "INVESTIGATOR";
    public const string AllEndings = "BETWEEN_TWO_WORLDS";
    public const string Completion = "COMPLETION_100";

    private static readonly Dictionary<string, string> DisplayNames =
        new Dictionary<string, string>
        {
            { Polyglot,     "Poliglot" },
            { Investigator, "Istrazitelj" },
            { AllEndings,   "Izmedu dva svijeta" },
            { Completion,   "100% zavrseno" }
        };

    private static readonly Dictionary<string, string> Descriptions =
        new Dictionary<string, string>
        {
            { Polyglot,     "Doci do kanonskog prijevoda svih rijeci u jednom prolazu." },
            { Investigator, "Otkriti svih 33 otkrica kroz vise prolaza." },
            { AllEndings,   "Otkljucati sva cetiri zavrsetka." },
            { Completion,   "Otkljucati Poliglota, Istrazitelja i sve zavrsetke." }
        };

    // Javlja se kad se otkljuca novi achievement (za popup).
    public static event Action<string> OnAchievementUnlocked;

    private static AchievementProgressData cached;

    private static AchievementProgressData Data
    {
        get
        {
            if (cached == null)
                cached = AchievementSaveSystem.Load();

            return cached;
        }
    }

    // Otkljucava achievement. Sigurno je pozvati vise puta.
    public static void Unlock(string id)
    {
        if (string.IsNullOrEmpty(id) || !DisplayNames.ContainsKey(id))
        {
            Debug.LogWarning("Unlock: nepoznat achievement: " + id);
            return;
        }

        if (Data.unlockedAchievements.Contains(id))
            return;

        Data.unlockedAchievements.Add(id);
        AchievementSaveSystem.Save(Data);

        Debug.Log("Achievement otkljucan: " + GetDisplayName(id));

        OnAchievementUnlocked?.Invoke(id);

        // Otkljucavanje jednog achievementa moze ispuniti uvjet za 100%
        CheckCompletion();
    }

    public static bool IsUnlocked(string id)
    {
        return !string.IsNullOrEmpty(id) && Data.unlockedAchievements.Contains(id);
    }

    public static string GetDisplayName(string id)
    {
        return DisplayNames.TryGetValue(id ?? "", out string name) ? name : id;
    }

    public static string GetDescription(string id)
    {
        return Descriptions.TryGetValue(id ?? "", out string d) ? d : "";
    }

    public static IEnumerable<string> AllAchievementIds => DisplayNames.Keys;

    public static int UnlockedCount => Data.unlockedAchievements.Count;

    public static int TotalCount => DisplayNames.Count;

    // Provjerava se nakon svakog otkljucavanja i nakon zavrsetka igre.
    public static void CheckCompletion()
    {
        if (IsUnlocked(Completion))
            return;

        if (IsUnlocked(Polyglot) &&
            IsUnlocked(Investigator) &&
            IsUnlocked(AllEndings))
        {
            Unlock(Completion);
        }
    }

    // Poziva se kad se otkljuca kraj, da provjeri 4/4.
    public static void CheckAllEndings()
    {
        if (!IsUnlocked(AllEndings) && EndingManager.AllEndingsUnlocked())
            Unlock(AllEndings);
    }

    public static void ResetAll()
    {
        cached = new AchievementProgressData();
        AchievementSaveSystem.ResetProgress();
    }
}
