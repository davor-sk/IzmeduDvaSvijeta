using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordCardDisplay : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI wordText;
    public GameObject[] choiceButtons; 

    public enum WordState { AUTO, IZBOR, NEPOZNATO }

    public Color autoColor = new Color(0.36f, 0.78f, 0.65f);   
    public Color izborColor = new Color(0.94f, 0.78f, 0.46f); 
    public Color nepoznatoColor = new Color(0.94f, 0.45f, 0.45f); 

    public void SetState(WordState state, string text, string[] choices = null)
    {
        wordText.text = text;

        switch (state)
        {
            case WordState.AUTO:
                background.color = autoColor;
                SetButtonsActive(false);
                break;
            case WordState.IZBOR:
                background.color = izborColor;
                SetButtonsActive(true);
                if (choices != null)
                {
                    for (int i = 0; i < choiceButtons.Length && i < choices.Length; i++)
                    {
                        var btnText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                        btnText.text = choices[i];
                    }
                }
                break;
            case WordState.NEPOZNATO:
                background.color = nepoznatoColor;
                wordText.text = "???";
                SetButtonsActive(false);
                break;
        }
    }

    private void SetButtonsActive(bool active)
    {
        foreach (var btn in choiceButtons)
            btn.SetActive(active);
    }
}