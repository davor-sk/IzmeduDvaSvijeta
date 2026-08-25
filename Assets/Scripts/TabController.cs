using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Button notebookTabButton;
    public Button clueTabButton;
    public GameObject notebookPanel;
    public GameObject clueLogPanel;

    public Color activeColor = new Color32(76, 175, 80, 255);
    public Color inactiveColor = new Color32(158, 158, 158, 255);

    void Start()
    {
        notebookTabButton.onClick.AddListener(ShowNotebook);
        clueTabButton.onClick.AddListener(ShowClueLog);
        ShowNotebook();
    }

    void ShowNotebook()
    {
        notebookPanel.SetActive(true);
        clueLogPanel.SetActive(false);
        SetTabColor(notebookTabButton, true);
        SetTabColor(clueTabButton, false);
    }

    void ShowClueLog()
    {
        notebookPanel.SetActive(false);
        clueLogPanel.SetActive(true);
        SetTabColor(notebookTabButton, false);
        SetTabColor(clueTabButton, true);
    }

    void SetTabColor(Button button, bool active)
    {
        var text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null) text.color = active ? activeColor : inactiveColor;
    }
    
}