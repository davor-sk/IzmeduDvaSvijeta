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

    public TextMeshProUGUI[] slotLabels;

    
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

        
        if (saveFeedbackText != null)
            saveFeedbackText.text = "";

        RefreshSlotLabels();
    }

    public void SaveToSlot1() { SaveToSlot(1); }
    public void SaveToSlot2() { SaveToSlot(2); }
    public void SaveToSlot3() { SaveToSlot(3); }

    public void SaveToSlot(int slot)
    {
        if (saveManager == null)
        {
            Debug.LogError("PauseMenu: nema GameSaveManagera.");
            return;
        }

        saveManager.SaveGame(slot);

        if (saveFeedbackText != null)
            saveFeedbackText.text = "Spremljeno u slot " + slot + ".";

        RefreshSlotLabels();
    }

    
    public void RefreshSlotLabels()
    {
        if (slotLabels == null)
            return;

        for (int i = 0; i < slotLabels.Length; i++)
        {
            if (slotLabels[i] != null)
                slotLabels[i].text = "Spremi " + SaveSystem.GetSlotLabel(i + 1);
        }
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
