using UnityEngine;
using TMPro;

// Brisanje trajnog napretka iz izbornika.
// Manageri su staticne klase pa ih Unity gumb ne moze zvati izravno;
// ova komponenta ih omotava u obicne metode koje se mogu spojiti na OnClick.
public class ProgressResetController : MonoBehaviour
{
    [Header("Osvjezavanje prikaza (neobavezno)")]
    // U MainMenu sceni obje sjede na ProgressPanelu.
    // AchievementsMenuDisplay prikazuje i otkrica i achievemente.
    public AchievementsMenuDisplay achievementsDisplay;
    public EndingsMenuDisplay endingsDisplay;

    [Header("Povratna poruka (neobavezno)")]
    public TextMeshProUGUI feedbackText;

    [Header("Sigurnosna potvrda")]
    public bool requireConfirmation = true;

    private string pendingAction = "";

    void OnEnable()
    {
        pendingAction = "";
        SetFeedback("");
    }

    public void ResetClues()
    {
        if (!Confirm("clues", "Obrisati sva otkrica? Klikni ponovno za potvrdu."))
            return;

        ClueManager.ResetAll();
        RefreshAll();
        SetFeedback("Otkrica obrisana.");
    }

    public void ResetAchievements()
    {
        if (!Confirm("achievements", "Obrisati sve achievemente? Klikni ponovno za potvrdu."))
            return;

        AchievementManager.ResetAll();
        RefreshAll();
        SetFeedback("Achievementi obrisani.");
    }

    public void ResetEndings()
    {
        if (!Confirm("endings", "Obrisati sve zavrsetke? Klikni ponovno za potvrdu."))
            return;

        EndingManager.ResetAll();
        RefreshAll();
        SetFeedback("Zavrseci obrisani.");
    }

    public void ResetEverything()
    {
        if (!Confirm("all", "Obrisati SAV napredak? Klikni ponovno za potvrdu."))
            return;

        ClueManager.ResetAll();
        AchievementManager.ResetAll();
        EndingManager.ResetAll();
        RefreshAll();
        SetFeedback("Sav napredak obrisan.");
    }

    // Brise i spremljene igre (slotove), uz sve ostalo.
    public void ResetEverythingIncludingSaves()
    {
        if (!Confirm("all_saves", "Obrisati napredak I spremljene igre? Klikni ponovno za potvrdu."))
            return;

        ClueManager.ResetAll();
        AchievementManager.ResetAll();
        EndingManager.ResetAll();

        for (int slot = 1; slot <= SaveSystem.SlotCount; slot++)
            SaveSystem.DeleteSave(slot);

        RefreshAll();
        SetFeedback("Sve obrisano, ukljucujuci spremljene igre.");
    }

    // Vraca true kad akciju treba stvarno izvrsiti.
    private bool Confirm(string action, string question)
    {
        if (!requireConfirmation)
            return true;

        if (pendingAction == action)
        {
            pendingAction = "";
            return true;
        }

        pendingAction = action;
        SetFeedback(question);
        return false;
    }

    private void RefreshAll()
    {
        // Ako polja nisu spojena, nadu se sami - panel je cesto neaktivan
        // pa se trazi i medu neaktivnima.
        if (achievementsDisplay == null)
            achievementsDisplay = FindFirstObjectByType<AchievementsMenuDisplay>(FindObjectsInactive.Include);

        if (endingsDisplay == null)
            endingsDisplay = FindFirstObjectByType<EndingsMenuDisplay>(FindObjectsInactive.Include);

        if (achievementsDisplay != null)
            achievementsDisplay.Refresh();

        if (endingsDisplay != null)
            endingsDisplay.Refresh();
    }

    private void SetFeedback(string text)
    {
        if (feedbackText != null)
            feedbackText.text = text;
    }
}
