using System.Text;
using UnityEngine;
using TMPro;

// Prikaz achievementa i otkrica u glavnom izborniku.
public class AchievementsMenuDisplay : MonoBehaviour
{
    [Header("Sazetak")]
    public TextMeshProUGUI cluesSummaryText;        // "OTKRICA: 12/33"
    public TextMeshProUGUI achievementsSummaryText; // "ACHIEVEMENTI: 1/4"

    [Header("Popis achievementa")]
    public TextMeshProUGUI achievementListText;     // jedan TMP blok sa svim achievementima

    [Header("Izgled")]
    public string cluesPrefix = "OTKRICA: ";
    public string achievementsPrefix = "ACHIEVEMENTI: ";
    public string unlockedMark = "[X] ";
    public string lockedMark = "[ ] ";

    void Start()
    {
        Refresh();
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (cluesSummaryText != null)
            cluesSummaryText.text = cluesPrefix + ClueManager.GetProgressText();

        if (achievementsSummaryText != null)
        {
            achievementsSummaryText.text = achievementsPrefix +
                AchievementManager.UnlockedCount + "/" + AchievementManager.TotalCount;
        }

        if (achievementListText != null)
        {
            var sb = new StringBuilder();

            foreach (var id in AchievementManager.AllAchievementIds)
            {
                bool unlocked = AchievementManager.IsUnlocked(id);

                sb.Append(unlocked ? unlockedMark : lockedMark);
                sb.Append(AchievementManager.GetDisplayName(id));
                sb.Append("\n");
                sb.Append("    ");
                sb.Append(AchievementManager.GetDescription(id));
                sb.Append("\n\n");
            }

            achievementListText.text = sb.ToString();
        }
    }
}
