using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[DefaultExecutionOrder(100)]
public class GameSaveManager : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public NotebookController notebook;

    public SceneTransitionController sceneTransition;

    public static bool ShouldLoadOnStart = false;

    public static int SlotToLoad = 1;

   
    private string currentNode = "";

    void Awake()
    {
        if (dialogueRunner == null)
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();

        if (notebook == null)
            notebook = ResolveNotebook();

        if (sceneTransition == null)
            sceneTransition = FindFirstObjectByType<SceneTransitionController>();

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
            LoadGame(SlotToLoad);
        }
    }

    private NotebookController ResolveNotebook()
    {
        var all = FindObjectsByType<NotebookController>(FindObjectsSortMode.None);

        if (all == null || all.Length == 0)
            return null;

        NotebookController best = all[0];

        foreach (var candidate in all)
        {
            if (candidate.GetAllWords().Count > best.GetAllWords().Count)
                best = candidate;
        }

        return best;
    }

    private void OnNodeStart(string nodeName)
    {
        currentNode = nodeName;
    }

    public void SaveGame(int slot)
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

            var floats = new Dictionary<string, float>();
            var strings = new Dictionary<string, string>();
            var bools = new Dictionary<string, bool>();

            foreach (var pair in all.FloatVariables)
                floats[pair.Key] = pair.Value;

            foreach (var pair in all.StringVariables)
                strings[pair.Key] = pair.Value;

            foreach (var pair in all.BoolVariables)
                bools[pair.Key] = pair.Value;

            foreach (var name in YarnVariableNames.FloatNames)
            {
                if (!floats.ContainsKey(name) && storage.TryGetValue<float>(name, out float v))
                    floats[name] = v;
            }

            foreach (var name in YarnVariableNames.StringNames)
            {
                if (!strings.ContainsKey(name) && storage.TryGetValue<string>(name, out string v))
                    strings[name] = v;
            }

            foreach (var name in YarnVariableNames.BoolNames)
            {
                if (!bools.ContainsKey(name) && storage.TryGetValue<bool>(name, out bool v))
                    bools[name] = v;
            }

            foreach (var pair in floats)
            {
                data.floatKeys.Add(pair.Key);
                data.floatValues.Add(pair.Value);
            }

            foreach (var pair in strings)
            {
                data.stringKeys.Add(pair.Key);
                data.stringValues.Add(pair.Value);
            }

            foreach (var pair in bools)
            {
                data.boolKeys.Add(pair.Key);
                data.boolValues.Add(pair.Value);
            }

            Debug.Log("SaveGame: spremljeno " + floats.Count + " float, " +
                      strings.Count + " string, " + bools.Count + " bool varijabli.");
        }

        var activeNotebook = ResolveNotebook() ?? notebook;

        if (activeNotebook != null)
            data.notebookWords = activeNotebook.GetAllWords();

        data.savedAtDisplay = System.DateTime.Now.ToString("dd.MM.yyyy. HH:mm");

        SaveSystem.Save(data, slot);
    }

    public void LoadGame(int slot)
    {
        var data = SaveSystem.Load(slot);

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

            Debug.Log("LoadGame: vraceno " + floats.Count + " float, " +
                      strings.Count + " string, " + bools.Count + " bool varijabli.");
        }

        var activeNotebook = ResolveNotebook() ?? notebook;

        if (activeNotebook != null)
            activeNotebook.RestoreWords(data.notebookWords);

        currentNode = data.currentNode;

        StartCoroutine(StartLoadedNode(data.currentNode));
    }

    private IEnumerator StartLoadedNode(string nodeName)
    {
        if (dialogueRunner.IsDialogueRunning)
            dialogueRunner.Stop().Forget();

        yield return null;

        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Stop().Forget();
            yield return null;
        }

        if (sceneTransition != null)
        {
            string session = NodeSessionMap.GetSessionFor(nodeName);

            if (!string.IsNullOrEmpty(session))
                sceneTransition.ApplyBackgroundForSession(session);
        }

        currentNode = nodeName;
        dialogueRunner.StartDialogue(nodeName).Forget();

        dialogueRunner.autoStart = wasAutoStart;

        Debug.Log("Igra ucitana, nastavak od nodea: " + nodeName);
    }
}
