using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class GameSaveController : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public NotebookController notebook;

    private readonly List<string> words = new List<string>();
    private readonly List<string> translations = new List<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterAutoAttach()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(
        UnityEngine.SceneManagement.Scene scene,
        UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        
        if (FindFirstObjectByType<DialogueRunner>() == null) return;
        if (FindFirstObjectByType<GameSaveController>() != null) return;

        new GameObject("GameSaveController").AddComponent<GameSaveController>();
    }

    private void Start()
    {
        if (dialogueRunner == null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        }

        if (notebook == null)
        {
            notebook = FindFirstObjectByType<NotebookController>();
        }

        if (dialogueRunner == null)
        {
            Debug.LogError("GameSaveController: DialogueRunner nije pronađen.");
            return;
        }

        RestoreIfContinuing();


        if (dialogueRunner.onNodeStart == null)
        {
            dialogueRunner.onNodeStart = new UnityEventString();
        }

        dialogueRunner.onNodeStart.AddListener(OnNodeStart);
    }

    private void OnDestroy()
    {
        if (dialogueRunner != null && dialogueRunner.onNodeStart != null)
        {
            dialogueRunner.onNodeStart.RemoveListener(OnNodeStart);
        }
    }

    private void RestoreIfContinuing()
    {
        if (!SaveSystem.ContinueRequested) return;

       
        SaveSystem.ContinueRequested = false;

        SaveData data = SaveSystem.Load();

        if (data == null)
        {
            Debug.LogWarning("Nastavak zatražen, ali spremljena igra ne postoji. Kreće se ispočetka.");
            return;
        }
        if (!dialogueRunner.Dialogue.NodeExists(data.nodeName))
        {
            Debug.LogWarning("Spremljeni čvor '" + data.nodeName + "' ne postoji. Kreće se ispočetka.");
            SaveSystem.Delete();
            return;
        }

        RestoreVariables(data);

        if (notebook != null)
        {
            notebook.RestoreEntries(data.notebookWords, data.notebookTranslations);
        }

        dialogueRunner.startNode = data.nodeName;

        Debug.Log("Igra nastavljena od čvora: " + data.nodeName);
    }

    private void RestoreVariables(SaveData data)
    {
        var floats = new Dictionary<string, float>();
        var strings = new Dictionary<string, string>();
        var bools = new Dictionary<string, bool>();

        for (int i = 0; i < Mathf.Min(data.floatKeys.Count, data.floatValues.Count); i++)
        {
            floats[data.floatKeys[i]] = data.floatValues[i];
        }

        for (int i = 0; i < Mathf.Min(data.stringKeys.Count, data.stringValues.Count); i++)
        {
            strings[data.stringKeys[i]] = data.stringValues[i];
        }

        for (int i = 0; i < Mathf.Min(data.boolKeys.Count, data.boolValues.Count); i++)
        {
            bools[data.boolKeys[i]] = data.boolValues[i];
        }

        dialogueRunner.VariableStorage.SetAllVariables(floats, strings, bools, true);
    }

    /// <summary>
    /// Okida se na početku svakog čvora, dakle i nakon svakog odgovora igrača.
    /// </summary>
    private void OnNodeStart(string nodeName)
    {
        SaveData data = new SaveData();
        data.nodeName = nodeName;

        var all = dialogueRunner.VariableStorage.GetAllVariables();

        foreach (var pair in all.FloatVariables)
        {
            data.floatKeys.Add(pair.Key);
            data.floatValues.Add(pair.Value);
        }

        foreach (var pair in all.StringVariables)
        {
            data.stringKeys.Add(pair.Key);
            data.stringValues.Add(pair.Value);
        }

        foreach (var pair in all.BoolVariables)
        {
            data.boolKeys.Add(pair.Key);
            data.boolValues.Add(pair.Value);
        }

        if (notebook != null)
        {
            notebook.GetEntries(words, translations);
            data.notebookWords.AddRange(words);
            data.notebookTranslations.AddRange(translations);
        }

        SaveSystem.Save(data);
    }
}
