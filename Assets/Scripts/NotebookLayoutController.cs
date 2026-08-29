using System.Collections;
using UnityEngine;

public class NotebookLayoutController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform leftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private CanvasGroup rightPanelCanvasGroup;

    [Header("Buttons")]
    [SerializeField] private GameObject showNotebookButton;
    [SerializeField] private GameObject closeNotebookButton;

    [Header("Settings")]
    [SerializeField] private bool startWithNotebookOpen = false;
    [SerializeField] private float animationDuration = 0.25f;

    private bool isNotebookOpen;
    private Coroutine animationCoroutine;

    private const float ClosedLeftPanelMaxX = 1f;
    private const float OpenLeftPanelMaxX = 0.62f;

    private void Start()
    {
        if (startWithNotebookOpen)
            SetOpenStateImmediate();
        else
            SetClosedStateImmediate();
    }

    public void ShowNotebook()
    {
        if (isNotebookOpen)
            return;

        isNotebookOpen = true;

        showNotebookButton.SetActive(false);
        closeNotebookButton.SetActive(true);

        rightPanel.SetActive(true);

        rightPanelCanvasGroup.alpha = 0f;
        rightPanelCanvasGroup.interactable = false;
        rightPanelCanvasGroup.blocksRaycasts = false;

        StartTransition(OpenLeftPanelMaxX, 1f, false);
    }

    public void HideNotebook()
    {
        if (!isNotebookOpen)
            return;

        isNotebookOpen = false;

        closeNotebookButton.SetActive(false);
        showNotebookButton.SetActive(true);

        rightPanelCanvasGroup.interactable = false;
        rightPanelCanvasGroup.blocksRaycasts = false;

        StartTransition(ClosedLeftPanelMaxX, 0f, true);
    }

    private void StartTransition(
        float targetLeftPanelMaxX,
        float targetNotebookAlpha,
        bool hideNotebookAfter)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(
            AnimateLayout(
                targetLeftPanelMaxX,
                targetNotebookAlpha,
                hideNotebookAfter
            )
        );
    }

    private IEnumerator AnimateLayout(
        float targetLeftPanelMaxX,
        float targetNotebookAlpha,
        bool hideNotebookAfter)
    {
        float startLeftPanelMaxX = leftPanel.anchorMax.x;
        float startNotebookAlpha = rightPanelCanvasGroup.alpha;

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / animationDuration);

            // Smoothstep
            t = t * t * (3f - 2f * t);

            float currentMaxX = Mathf.Lerp(
                startLeftPanelMaxX,
                targetLeftPanelMaxX,
                t
            );

            leftPanel.anchorMax = new Vector2(
                currentMaxX,
                1f
            );

            // Zadržavamo originalne margine
            leftPanel.offsetMin = new Vector2(60f, 60f);
            leftPanel.offsetMax = new Vector2(-60f, -120f);

            rightPanelCanvasGroup.alpha = Mathf.Lerp(
                startNotebookAlpha,
                targetNotebookAlpha,
                t
            );

            yield return null;
        }

        // Osiguraj potpuno precizno završno stanje
        leftPanel.anchorMax = new Vector2(
            targetLeftPanelMaxX,
            1f
        );

        leftPanel.offsetMin = new Vector2(60f, 60f);
        leftPanel.offsetMax = new Vector2(-60f, -120f);

        rightPanelCanvasGroup.alpha = targetNotebookAlpha;

        if (!hideNotebookAfter)
        {
            rightPanelCanvasGroup.interactable = true;
            rightPanelCanvasGroup.blocksRaycasts = true;
        }

        animationCoroutine = null;
    }

    private void SetOpenStateImmediate()
    {
        isNotebookOpen = true;

        leftPanel.anchorMin = new Vector2(0f, 0f);
        leftPanel.anchorMax = new Vector2(OpenLeftPanelMaxX, 1f);
        leftPanel.offsetMin = new Vector2(60f, 60f);
        leftPanel.offsetMax = new Vector2(-60f, -120f);

        rightPanel.SetActive(true);

        rightPanelCanvasGroup.alpha = 1f;
        rightPanelCanvasGroup.interactable = true;
        rightPanelCanvasGroup.blocksRaycasts = true;

        showNotebookButton.SetActive(false);
        closeNotebookButton.SetActive(true);
    }

    private void SetClosedStateImmediate()
    {
        isNotebookOpen = false;

        leftPanel.anchorMin = new Vector2(0f, 0f);
        leftPanel.anchorMax = new Vector2(ClosedLeftPanelMaxX, 1f);
        leftPanel.offsetMin = new Vector2(60f, 60f);
        leftPanel.offsetMax = new Vector2(-60f, -120f);

        rightPanelCanvasGroup.alpha = 0f;
        rightPanelCanvasGroup.interactable = false;
        rightPanelCanvasGroup.blocksRaycasts = false;

        showNotebookButton.SetActive(true);
        closeNotebookButton.SetActive(false);
    }
}