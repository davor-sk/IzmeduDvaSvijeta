using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Yarn.Unity;

public class ClueLogController : MonoBehaviour
{
    public Transform clueListContent;
    public GameObject clueEntryPrefab;
    public TMP_FontAsset clueFont;

    private HashSet<string> unlockedClues = new HashSet<string>();

    [YarnCommand("add_clue")]
    public void AddClue(string clueId, string clueText)
    {
        if (unlockedClues.Contains(clueId))
        {
            return;
        }

        unlockedClues.Add(clueId);

        GameObject entry = Instantiate(clueEntryPrefab, clueListContent);
        var text = entry.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = clueText;
            text.font = clueFont;
        }

        Debug.Log("Novi trag otključan: " + clueId);
    }

    public bool HasClue(string clueId)
    {
        return unlockedClues.Contains(clueId);
    }
}