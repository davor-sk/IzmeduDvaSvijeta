using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Yarn.Unity;

// Prikaz otkrica u clue log panelu.
// Nema vlastito stanje - ClueManager je jedini izvor istine, a ovo je samo pogled na njega.
// Zato se popis ispravno popuni i kad su otkrica skupljena u ranijem playthroughu.
public class ClueLogController : MonoBehaviour
{
    public Transform clueListContent;
    public GameObject clueEntryPrefab;
    public TMP_FontAsset clueFont;

    private readonly Dictionary<string, GameObject> entries = new Dictionary<string, GameObject>();

    void OnEnable()
    {
        ClueManager.OnClueUnlocked += HandleClueUnlocked;
        Rebuild();
    }

    void OnDisable()
    {
        ClueManager.OnClueUnlocked -= HandleClueUnlocked;
    }

    // Popuni popis iz trajnog napretka (ukljucujuci ranije playthroughove).
    public void Rebuild()
    {
        if (clueListContent == null || clueEntryPrefab == null)
            return;

        foreach (var entry in entries.Values)
        {
            if (entry != null)
                Destroy(entry);
        }

        entries.Clear();

        foreach (var clue in ClueManager.AllClues)
        {
            if (ClueManager.IsUnlocked(clue.id))
                AddEntry(clue.id);
        }
    }

    private void HandleClueUnlocked(string clueId)
    {
        AddEntry(clueId);
    }

    private void AddEntry(string clueId)
    {
        if (string.IsNullOrEmpty(clueId) || entries.ContainsKey(clueId))
            return;

        if (clueListContent == null || clueEntryPrefab == null)
            return;

        GameObject entry = Instantiate(clueEntryPrefab, clueListContent);

        var text = entry.GetComponentInChildren<TextMeshProUGUI>();

        if (text != null)
        {
            text.text = ClueManager.GetDescription(clueId);

            if (clueFont != null)
                text.font = clueFont;
        }

        entries[clueId] = entry;
    }

    public bool HasClue(string clueId)
    {
        return ClueManager.IsUnlocked(clueId);
    }

    // Zadrzano zbog kompatibilnosti sa starim <<add_clue>> pozivima u TestSessions.yarn.
    // Otkrica u pravim sesijama idu kroz <<unlock_clue>>.
    [YarnCommand("add_clue")]
    public void AddClue(string clueId, string clueText)
    {
        ClueManager.AddClue(clueId);
    }
}
