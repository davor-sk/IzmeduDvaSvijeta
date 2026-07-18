 using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    [YarnCommand("show_ending")]
    public void ShowEnding(string quote)
    {        
        PauseMenuController.CanPause = false;

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
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PauseMenuController.CanPause = true;

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}