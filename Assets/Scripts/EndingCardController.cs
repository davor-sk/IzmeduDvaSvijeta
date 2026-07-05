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
        
        if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Stop();
        }

        endingCardPanel.SetActive(true);
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}