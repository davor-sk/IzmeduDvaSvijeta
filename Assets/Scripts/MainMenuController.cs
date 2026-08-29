using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    // Gumb "Nastavi igru" - sakriva se ako nema spremljene igre
    public GameObject continueButton;

    // Neobavezno: prikaz kada je igra spremljena
    public TextMeshProUGUI saveInfoText;

    void Start()
    {
        RefreshContinueButton();
    }

    private void RefreshContinueButton()
    {
        bool hasSave = SaveSystem.HasSave();

        if (continueButton != null)
            continueButton.SetActive(hasSave);

        if (saveInfoText != null)
        {
            if (hasSave)
            {
                var data = SaveSystem.Load();
                saveInfoText.text = data != null && !string.IsNullOrEmpty(data.savedAtDisplay)
                    ? "Spremljeno: " + data.savedAtDisplay
                    : "";
            }
            else
            {
                saveInfoText.text = "";
            }
        }
    }

    public void StartNewGame()
    {
        // nova igra uvijek krece od pocetka, ne od spremljenog nodea
        GameSaveManager.ShouldLoadOnStart = false;
        SceneManager.LoadScene("SampleScene");
    }

    // Poziva ga ContinueButton
    public void ContinueGame()
    {
        if (!SaveSystem.HasSave())
        {
            Debug.LogWarning("Nema spremljene igre.");
            RefreshContinueButton();
            return;
        }

        GameSaveManager.ShouldLoadOnStart = true;
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
