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

    // Vraca sve rijeci iz biljeznice, redoslijedom dodavanja, za spremanje igre
    public List<NotebookWordEntry> GetAllWords()
    {
        var result = new List<NotebookWordEntry>();

        foreach (var pair in wordTranslations)
        {
            result.Add(new NotebookWordEntry
            {
                dhornWord = pair.Key,
                translation = pair.Value.text
            });
        }

        return result;
    }

    // Ponovno gradi biljeznicu iz spremljene igre
    public void RestoreWords(List<NotebookWordEntry> words)
    {
        ClearNotebook();

        if (words == null)
            return;

        foreach (var word in words)
        {
            AddWord(word.dhornWord, word.translation);
        }
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