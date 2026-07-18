using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Yarn.Unity;

public class NotebookController : MonoBehaviour
{
    public Transform wordListContent;
    public GameObject notebookEntryPrefab;
    public TMP_FontAsset dhornWordFont;
    public TMP_FontAsset translationFont;

    private Dictionary<string, TextMeshProUGUI> wordTranslations =
        new Dictionary<string, TextMeshProUGUI>();

    [YarnCommand("add_word")]
    public void AddWord(string dhornWord, string translation)
    {
        if (wordTranslations.ContainsKey(dhornWord))
        {
            wordTranslations[dhornWord].text = translation;

            Debug.Log(
                "Ažurirana riječ: " +
                dhornWord +
                " = " +
                translation
            );

            return;
        }

        GameObject entry = Instantiate(
            notebookEntryPrefab,
            wordListContent
        );

        var texts = entry.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length >= 2)
        {
            texts[0].text = dhornWord;
            texts[0].font = dhornWordFont;

            texts[1].text = translation;
            texts[1].font = translationFont;

            wordTranslations.Add(dhornWord, texts[1]);
        }

        Debug.Log(
            "Dodana riječ: " +
            dhornWord +
            " = " +
            translation
        );
    }

    public void ClearNotebook()
    {
        foreach (Transform child in wordListContent)
        {
            Destroy(child.gameObject);
        }

        wordTranslations.Clear();
    }
}