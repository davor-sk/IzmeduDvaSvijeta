using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
   
    public GameObject continueButton;

   
    public GameObject[] slotButtons;

   
    public TextMeshProUGUI[] slotLabels;

  
    public TextMeshProUGUI saveInfoText;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1.5f;

    private bool isTransitioning = false;

    void Start()
    {
        RefreshContinueButton();
    }

    private void RefreshContinueButton()
    {
        bool hasAnySave = SaveSystem.HasAnySave();

        if (continueButton != null)
            continueButton.SetActive(hasAnySave);

       
        for (int i = 0; i < SaveSystem.SlotCount; i++)
        {
            bool slotHasSave = SaveSystem.HasSave(i + 1);

            if (slotLabels != null && i < slotLabels.Length && slotLabels[i] != null)
                slotLabels[i].text = "NASTAVI " + SaveSystem.GetSlotLabel(i + 1);

            if (slotButtons != null && i < slotButtons.Length && slotButtons[i] != null)
                slotButtons[i].SetActive(slotHasSave);
        }

        if (saveInfoText != null)
            saveInfoText.text = "";
    }

    public void StartNewGame()
    {
        if (isTransitioning)
            return;

        GameSaveManager.ShouldLoadOnStart = false;
        StartCoroutine(LoadGameSceneWithFade());
    }

    public void ContinueSlot1() { ContinueGame(1); }
    public void ContinueSlot2() { ContinueGame(2); }
    public void ContinueSlot3() { ContinueGame(3); }

   
    public void ContinueGame()
    {
        for (int slot = 1; slot <= SaveSystem.SlotCount; slot++)
        {
            if (SaveSystem.HasSave(slot))
            {
                ContinueGame(slot);
                return;
            }
        }

        Debug.LogWarning("Nema spremljene igre.");
        RefreshContinueButton();
    }

    public void ContinueGame(int slot)
    {
        if (isTransitioning)
            return;

        if (!SaveSystem.HasSave(slot))
        {
            Debug.LogWarning("Slot " + slot + " je prazan.");
            RefreshContinueButton();
            return;
        }

        GameSaveManager.ShouldLoadOnStart = true;
        GameSaveManager.SlotToLoad = slot;

        StartCoroutine(LoadGameSceneWithFade());
    }

    private IEnumerator LoadGameSceneWithFade()
    {
        isTransitioning = true;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;

            float startAlpha = fadeCanvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / fadeDuration);
                t = t * t * (3f - 2f * t);

                fadeCanvasGroup.alpha = Mathf.Lerp(
                    startAlpha,
                    1f,
                    t
                );

                yield return null;
            }

            fadeCanvasGroup.alpha = 1f;
        }

        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
