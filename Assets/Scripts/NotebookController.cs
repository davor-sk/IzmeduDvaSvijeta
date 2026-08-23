using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Yarn.Unity;

public class NotebookController : MonoBehaviour
{
    public Transform wordListContent;
    public GameObject notebookEntryPrefab;

    private Dictionary<string, TextMeshProUGUI> wordTranslations =
        new Dictionary<string, TextMeshProUGUI>();

    [YarnCommand("add_word")]
    public void AddWord(string dhornWord, string translation)
    {
        // Ako riječ već postoji, samo ažuriraj prijevod
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

        // Ako riječ još ne postoji, napravi novi unos
        GameObject entry = Instantiate(
            notebookEntryPrefab,
            wordListContent
        );

        var texts = entry.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length >= 2)
        {
            texts[0].text = dhornWord;
            texts[1].text = translation;

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

    public void GetEntries(List<string> words, List<string> translations)
    {
        words.Clear();
        translations.Clear();

        foreach (var pair in wordTranslations)
        {
            if (pair.Value == null) continue;

            words.Add(pair.Key);
            translations.Add(pair.Value.text);
        }
    }


    public void RestoreEntries(List<string> words, List<string> translations)
    {
        if (words == null || translations == null) return;

        ClearNotebook();

        int count = Mathf.Min(words.Count, translations.Count);

        for (int i = 0; i < count; i++)
        {
            AddWord(words[i], translations[i]);
        }
    }
}