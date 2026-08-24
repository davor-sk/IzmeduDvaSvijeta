using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    // Spremanje igre
    public GameSaveManager saveManager;
    public TextMeshProUGUI saveFeedbackText;

    
    public static bool CanPause = true;

    private bool isPaused = false;

    void Start()
    {
       
        Time.timeScale = 1f;
        CanPause = true;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (saveManager == null)
            saveManager = FindFirstObjectByType<GameSaveManager>();

        if (saveFeedbackText != null)
            saveFeedbackText.text = "";
    }

    void Update()
    {
        if (!CanPause)
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        // svaki put kad se otvori pauza, poruka o spremanju krece prazna
        if (saveFeedbackText != null)
            saveFeedbackText.text = "";
    }

    // Poziva ga SaveButton u PauseMenuPanelu
    public void SaveGame()
    {
        if (saveManager == null)
        {
            Debug.LogError("PauseMenu: nema GameSaveManagera.");
            return;
        }

        saveManager.SaveGame();

        if (saveFeedbackText != null)
            saveFeedbackText.text = "Igra spremljena.";
    }

    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

   
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

   
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
