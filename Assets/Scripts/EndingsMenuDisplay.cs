using UnityEngine;
using TMPro;


public class EndingsMenuDisplay : MonoBehaviour
{
    [Header("Sazetak")]
    public TextMeshProUGUI summaryText;
    public string summaryPrefix = "ENDINGS: ";

    [Header("Pojedinacni krajevi (redom: dhorn, government, truth, war)")]
    public TextMeshProUGUI[] endingLabels;

    [Header("Izgled")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = new Color(1f, 1f, 1f, 0.35f);

    
    public bool hideNamesOfLockedEndings = false;
    public string lockedPlaceholder = "???";

    void Start()
    {
        Refresh();
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (summaryText != null)
            summaryText.text = summaryPrefix + EndingManager.GetProgressText();

        if (endingLabels == null)
            return;

        var ids = EndingManager.AllEndingIds;

        for (int i = 0; i < endingLabels.Length && i < ids.Length; i++)
        {
            var label = endingLabels[i];

            if (label == null)
                continue;

            string id = ids[i];
            bool unlocked = EndingManager.IsUnlocked(id);

            string name = unlocked || !hideNamesOfLockedEndings
                ? EndingManager.GetDisplayName(id)
                : lockedPlaceholder;

            label.text = unlocked
                ? name + " - odigrano"
                : name + " - nije odigrano";

            label.color = unlocked ? unlockedColor : lockedColor;
        }
    }
}
