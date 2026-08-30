using UnityEngine;
using TMPro;

public class ClueSummaryMenuDisplay : MonoBehaviour
{
    public TextMeshProUGUI summaryText;
    public string prefix = "DOSJE: ";
    private const int TotalClueCount = 33;

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
        var data = ClueSaveSystem.Load();
        int count = data.unlockedClues != null ? data.unlockedClues.Count : 0;
        if (summaryText != null)
            summaryText.text = $"{prefix}{count} / {TotalClueCount}";
    }
}