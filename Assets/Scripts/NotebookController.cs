using UnityEngine;
using TMPro;
using Yarn.Unity;

public class NotebookController : MonoBehaviour
{
    public Transform wordListContent;
    public GameObject notebookEntryPrefab;

    void Start()
    {
        AddWord("vel-ahn", "razumijemo");
        AddWord("moran", "dom");
        AddWord("nem-dah", "bez straha");
        AddWord("vel-ahn", "razumijemo");
        AddWord("moran", "dom");
        AddWord("nem-dah", "bez straha");
        AddWord("vel-ahn", "razumijemo");
        AddWord("moran", "dom");
        AddWord("nem-dah", "bez straha");
    }

    [YarnCommand("add_word")]
    public void AddWord(string dhornWord, string translation)
    {
        GameObject entry = Instantiate(notebookEntryPrefab, wordListContent);
        
        var texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 2)
        {
            texts[0].text = dhornWord;
            texts[1].text = translation;
        }
        
        Debug.Log("Dodana riječ: " + dhornWord + " = " + translation);
    }

    public void ClearNotebook()
    {
        foreach (Transform child in wordListContent)
        {
            Destroy(child.gameObject);
        }
    }
}