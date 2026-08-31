using System.Collections;
using UnityEngine;
using Yarn.Unity;
public class SceneTransitionController : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [Header("Yarn")]
    [SerializeField] private DialogueRunner dialogueRunner;
    [System.Serializable]
    public class SessionBackground
    {
        public string sessionName;
        public Sprite background;
    }

    [Header("Background")]
    [SerializeField] private UnityEngine.UI.Image backgroundImage;
    [SerializeField] private SessionBackground[] sessionBackgrounds;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;
    }

    public void FadeOut()
    {
        StartFade(1f);
    }

    public void FadeIn()
    {
        StartFade(0f);
    }

    [YarnCommand("session_transition")]
    public void SessionTransition(string nextSession)
    {
        StartCoroutine(SessionTransitionRoutine(nextSession));
    }

    private IEnumerator SessionTransitionRoutine(string nextSession)
    {
        // Fade u crno
        yield return FadeRoutine(1f);

        // Promijeni background dok je ekran crn
        foreach (var item in sessionBackgrounds)
        {
            if (item.sessionName == nextSession && item.background != null)
            {
                backgroundImage.sprite = item.background;
                break;
            }
        }

        // Pokreni sljedeći session
        dialogueRunner.Stop();
        dialogueRunner.StartDialogue(nextSession);

        // Fade natrag u igru
        yield return FadeRoutine(0f);
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);
            t = t * t * (3f - 2f * t);

            fadeCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                t
            );

            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        fadeCoroutine = null;
    }
}