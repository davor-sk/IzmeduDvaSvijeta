using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Popup koji se pojavi kad se otkljuca achievement ili otkrice.
// Koristi red cekanja jer se dva otkrica mogu otkljucati u istom trenutku
// (npr. C27 i C28 su u Session6 jedan ispod drugoga).
public class AchievementPopup : MonoBehaviour
{
    [Header("Reference")]
    public GameObject popupPanel;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public GameAudioManager audioManager;

    [Header("Trajanje")]
    public float fadeInDuration = 0.4f;
    public float holdDuration = 2.5f;
    public float fadeOutDuration = 0.6f;

    [Header("Prikazivati i otkrica?")]
    public bool showCluePopups = true;
    public string clueTitle = "NOVO OTKRICE";

    private readonly Queue<(string title, string description)> queue =
        new Queue<(string, string)>();

    private bool isShowing = false;

    void OnEnable()
    {
        AchievementManager.OnAchievementUnlocked += HandleAchievement;
        ClueManager.OnClueUnlocked += HandleClue;
    }

    void OnDisable()
    {
        AchievementManager.OnAchievementUnlocked -= HandleAchievement;
        ClueManager.OnClueUnlocked -= HandleClue;
    }

    void Awake()
    {
        // Ako polja nisu rucno povezana u Inspectoru, pokusaj ih naci sam.
        if (popupPanel == null)
            popupPanel = gameObject;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // CanvasGroup je nuzan za fade - ako ga nema, dodaj ga.
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (audioManager == null)
            audioManager = FindFirstObjectByType<GameAudioManager>();

        // Ako tekstovi nisu povezani, uzmi ih iz djece.
        var texts = GetComponentsInChildren<TextMeshProUGUI>(true);

        if (titleText == null && texts.Length > 0)
            titleText = texts[0];

        if (descriptionText == null && texts.Length > 1)
            descriptionText = texts[1];
    }

    void Start()
    {
        canvasGroup.alpha = 0f;

        // Ako je panel isti objekt na kojem je skripta, NE gasimo ga -
        // time bi se ugasila i skripta pa popup nikad ne bi radio.
        // Dovoljno je da je proziran i da ne hvata klikove.
        if (popupPanel != gameObject)
            popupPanel.SetActive(false);
        else
            SetInteractable(false);
    }

    private void SetInteractable(bool state)
    {
        canvasGroup.blocksRaycasts = state;
        canvasGroup.interactable = state;
    }

    private void HandleAchievement(string id)
    {
        Enqueue(
            "ACHIEVEMENT: " + AchievementManager.GetDisplayName(id),
            AchievementManager.GetDescription(id)
        );
    }

    private void HandleClue(string clueId)
    {
        if (!showCluePopups)
            return;

        Enqueue(
            clueTitle + " (" + ClueManager.GetProgressText() + ")",
            ClueManager.GetDescription(clueId)
        );
    }

    private void Enqueue(string title, string description)
    {
        queue.Enqueue((title, description));

        if (!isShowing)
            StartCoroutine(ShowQueue());
    }

    private IEnumerator ShowQueue()
    {
        isShowing = true;

        while (queue.Count > 0)
        {
            var item = queue.Dequeue();
            yield return StartCoroutine(ShowOne(item.title, item.description));
        }

        isShowing = false;
    }

    private IEnumerator ShowOne(string title, string description)
    {
        if (popupPanel == null || canvasGroup == null)
            yield break;

        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        if (popupPanel != gameObject)
            popupPanel.SetActive(true);

        SetInteractable(true);

        if (audioManager != null)
            audioManager.PlayNotificationBeep();

        yield return Fade(0f, 1f, fadeInDuration);

        yield return new WaitForSeconds(holdDuration);

        yield return Fade(1f, 0f, fadeOutDuration);

        SetInteractable(false);

        if (popupPanel != gameObject)
            popupPanel.SetActive(false);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
