using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Neobavezno - ako se ostavi prazno, traži se po imenu")]
    public Button newGameButton;

    [Header("Neobavezno - ako se ostavi prazno, gumb se stvara u kodu")]
    public Button continueButton;

    private bool awaitingNewGameConfirm = false;
    private string newGameLabelOriginal = "NOVA IGRA";
    private TextMeshProUGUI newGameLabel;

    private void Start()
    {
        if (newGameButton == null)
        {
            GameObject found = GameObject.Find("NewGameButton");
            if (found != null) newGameButton = found.GetComponent<Button>();
        }

        if (newGameButton != null)
        {
            newGameLabel = newGameButton.GetComponentInChildren<TextMeshProUGUI>();
            if (newGameLabel != null) newGameLabelOriginal = newGameLabel.text;

            
            DisablePersistentCalls(newGameButton);
            newGameButton.onClick.RemoveListener(OnNewGameClicked);
            newGameButton.onClick.AddListener(OnNewGameClicked);
        }

        SetUpContinueButton();
    }

    private void SetUpContinueButton()
    {
        bool hasSave = SaveSystem.HasSave();

        if (continueButton == null && hasSave && newGameButton != null)
        {
            continueButton = CreateContinueButton();
        }

        if (continueButton == null) return;

        
        continueButton.gameObject.SetActive(hasSave);

        DisablePersistentCalls(continueButton);
        continueButton.onClick.RemoveListener(ContinueGame);
        continueButton.onClick.AddListener(ContinueGame);
    }


    private Button CreateContinueButton()
    {
        GameObject clone = Instantiate(
            newGameButton.gameObject,
            newGameButton.transform.parent
        );

        clone.name = "ContinueButton";

  
        Button button = clone.GetComponent<Button>();
        DisablePersistentCalls(button);
        button.onClick.RemoveAllListeners();

        var label = clone.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = "NASTAVI";

        
        clone.transform.SetSiblingIndex(newGameButton.transform.GetSiblingIndex());

        return button;
    }

    private void DisablePersistentCalls(Button button)
    {
        if (button == null) return;

        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            button.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
        }
    }


    private void OnNewGameClicked()
    {
        if (SaveSystem.HasSave() && !awaitingNewGameConfirm)
        {
            awaitingNewGameConfirm = true;

            if (newGameLabel != null)
            {
                newGameLabel.text = "IZBRISATI NAPREDAK? KLIKNI PONOVNO";
            }

            return;
        }

        if (newGameLabel != null) newGameLabel.text = newGameLabelOriginal;
        awaitingNewGameConfirm = false;

        StartNewGame();
    }

    [Preserve]
    public void StartNewGame()
    {
        SaveSystem.Delete();
        SaveSystem.ContinueRequested = false;

        SceneManager.LoadScene("SampleScene");
    }

    [Preserve]
    public void ContinueGame()
    {
        if (!SaveSystem.HasSave())
        {
            Debug.LogWarning("Nema spremljene igre za nastavak.");
            return;
        }

        SaveSystem.ContinueRequested = true;

        SceneManager.LoadScene("SampleScene");
    }

    [Preserve]
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
