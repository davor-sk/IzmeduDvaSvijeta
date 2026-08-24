using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;


public class GameSaveManager : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public NotebookController notebook;


    public static bool ShouldLoadOnStart = false;

   
    private string currentNode = "";

    void Awake()
    {
        if (dialogueRunner == null)
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();

        if (notebook == null)
            notebook = FindFirstObjectByType<NotebookController>();

        if (ShouldLoadOnStart && dialogueRunner != null)
        {
            wasAutoStart = dialogueRunner.autoStart;
            dialogueRunner.autoStart = false;
        }
    }

    private bool wasAutoStart = false;

    void OnEnable()
    {
        if (dialogueRunner != null && dialogueRunner.onNodeStart != null)
            dialogueRunner.onNodeStart.AddListener(OnNodeStart);
    }

    void OnDisable()
    {
        if (dialogueRunner != null && dialogueRunner.onNodeStart != null)
            dialogueRunner.onNodeStart.RemoveListener(OnNodeStart);
    }

    void Start()
    {
        if (ShouldLoadOnStart)
        {
            ShouldLoadOnStart = false;
            LoadGame();

            if (dialogueRunner != null)
                dialogueRunner.autoStart = wasAutoStart;
        }
    }

    private void OnNodeStart(string nodeName)
    {
        currentNode = nodeName;
    }

    public void SaveGame()
    {
        if (dialogueRunner == null)
        {
            Debug.LogError("SaveGame: nema DialogueRunnera.");
            return;
        }

        if (string.IsNullOrEmpty(currentNode))
        {
            Debug.LogWarning("SaveGame: dijalog jos nije zapoceo, nema se sto spremiti.");
            return;
        }

        var data = new SaveData();
        data.currentNode = currentNode;

        var storage = dialogueRunner.VariableStorage;
        if (storage != null)
        {
            var all = storage.GetAllVariables();

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
        }

        if (notebook != null)
            data.notebookWords = notebook.GetAllWords();

        data.savedAtDisplay = System.DateTime.Now.ToString("dd.MM.yyyy. HH:mm");

        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        var data = SaveSystem.Load();

        if (data == null)
        {
            Debug.LogWarning("LoadGame: nema spremljene igre.");
            return;
        }

        if (dialogueRunner == null)
        {
            Debug.LogError("LoadGame: nema DialogueRunnera.");
            return;
        }

        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Stop().Forget();
        }

        var storage = dialogueRunner.VariableStorage;
        if (storage != null)
        {
            var floats = new Dictionary<string, float>();
            var strings = new Dictionary<string, string>();
            var bools = new Dictionary<string, bool>();

            for (int i = 0; i < data.floatKeys.Count; i++)
                floats[data.floatKeys[i]] = data.floatValues[i];

            for (int i = 0; i < data.stringKeys.Count; i++)
                strings[data.stringKeys[i]] = data.stringValues[i];

            for (int i = 0; i < data.boolKeys.Count; i++)
                bools[data.boolKeys[i]] = data.boolValues[i];

            storage.SetAllVariables(floats, strings, bools, true);
        }

        if (notebook != null)
            notebook.RestoreWords(data.notebookWords);

        currentNode = data.currentNode;
        dialogueRunner.StartDialogue(data.currentNode).Forget();

        Debug.Log("Igra ucitana, nastavak od nodea: " + data.currentNode);
    }
}
