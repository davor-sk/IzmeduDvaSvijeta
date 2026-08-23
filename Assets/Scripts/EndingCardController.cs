 using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using TMPro;
using Yarn.Unity;

public class EndingCardController : MonoBehaviour
{
    public GameObject endingCardPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI endingQuote;
    public GameObject mainMenuButton;
    public CanvasGroup canvasGroup;
    public DialogueRunner dialogueRunner;

    private bool isLoadingMenu = false;

    [YarnCommand("show_ending")]
    public void ShowEnding(string quote)
    {
        Debug.Log("EndingCardController.ShowEnding called: " + quote);

        
        PauseMenuController.CanPause = false;

        SaveSystem.Delete();

        endingCardPanel.SetActive(true);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        endingQuote.text = quote;
        mainMenuButton.SetActive(false);

        StartCoroutine(FadeInAndShowButton());
    }

    private IEnumerator FadeInAndShowButton()
    {
        canvasGroup.alpha = 0f;
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = elapsed / duration;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(2.5f);

        mainMenuButton.SetActive(true);

        var btn = mainMenuButton.GetComponentInChildren<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveListener(GoToMainMenu);
            btn.onClick.AddListener(GoToMainMenu);
        }
    }

    [Preserve]
    public void GoToMainMenu()
    {
       
        if (isLoadingMenu) return;
        isLoadingMenu = true;

        Debug.Log("EndingCardController.GoToMainMenu -> loading MainMenu");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}