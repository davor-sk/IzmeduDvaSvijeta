using UnityEngine;
using Yarn.Unity;

// Prati je li igrac u OVOM prolazu dosao do kanonskog prijevoda svih rijeci.
// Provjera se radi nakon svake promjene u biljeznici, pa se achievement
// otkljucava u trenutku kad zadnja rijec sjedne na mjesto - ne tek na kraju igre.
//
// Ispravljanje ranije pogreske je namjerna mehanika i NE smeta:
// gleda se konacno stanje varijabli, ne redoslijed pogadanja.
// Kad je jednom otkljucan, kasnija promjena prijevoda ga ne oduzima.
public class PolyglotWatcher : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public NotebookController notebook;

    void Awake()
    {
        if (dialogueRunner == null)
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();

        if (notebook == null)
            notebook = FindFirstObjectByType<NotebookController>();
    }

    void OnEnable()
    {
        NotebookController.OnWordChanged += HandleWordChanged;
    }

    void OnDisable()
    {
        NotebookController.OnWordChanged -= HandleWordChanged;
    }

    private void HandleWordChanged()
    {
        CheckPolyglot();
    }

    public void CheckPolyglot()
    {
        if (AchievementManager.IsUnlocked(AchievementManager.Polyglot))
            return;

        if (dialogueRunner == null)
            return;

        var storage = dialogueRunner.VariableStorage;

        if (storage == null)
            return;

        if (TranslationChecker.AllCorrect(storage))
            AchievementManager.Unlock(AchievementManager.Polyglot);
    }
}
