#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class OptionsPresenter : DialoguePresenterBase
{
    public Transform? optionsContainer;
    public GameObject? optionButtonPrefab;

    // NOVO — reference za layout integraciju
    public LayoutElement? optionsBoxLayoutElement;
    public RectTransform? dialogueContainerRect;

    private List<GameObject> spawnedButtons = new List<GameObject>();
    private int selectedIndex = -1;

    public float buttonWidth = 180f;
    public float buttonHeight = 50f;
    public float buttonSpacing = 10f;
    public float buttonHorizontalPadding = 20f;

    public override YarnTask OnDialogueStartedAsync() => YarnTask.CompletedTask;
    public override YarnTask OnDialogueCompleteAsync() { ClearButtons(); return YarnTask.CompletedTask; }
    public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token) => YarnTask.CompletedTask;

    public override async YarnTask<DialogueOption?> RunOptionsAsync(
        DialogueOption[] dialogueOptions,
        LineCancellationToken cancellationToken)
    {
        ClearButtons();
        selectedIndex = -1;

        for (int i = 0; i < dialogueOptions.Length; i++)
        {
            var buttonObj = Instantiate(optionButtonPrefab, optionsContainer);
            if (buttonObj == null) continue;
            spawnedButtons.Add(buttonObj);

            var rect = buttonObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0.5f, 1f);
                rect.sizeDelta = new Vector2(-buttonHorizontalPadding * 2f, buttonHeight);
                rect.anchoredPosition = new Vector2(0, -i * (buttonHeight + buttonSpacing));
            }

            var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = dialogueOptions[i].Line.TextWithoutCharacterName.Text;
                buttonText.color = Color.white;
                buttonText.fontSize = 18;
                buttonText.textWrappingMode = TextWrappingModes.Normal;
                buttonText.overflowMode = TextOverflowModes.Ellipsis;
            }

            var button = buttonObj.GetComponentInChildren<Button>();
            int capturedIndex = i;

            if (button != null)
            {
                button.interactable = false;
                button.onClick.AddListener(() => {
                    selectedIndex = capturedIndex;
                    Debug.Log("Odabran index: " + capturedIndex);
                });
            }
        }

        // NOVO — javi DialogueContainer Vertical Layout Groupu koliko je OptionsBox visok,
        // pa se cijeli blok (DialogueBox + OptionsBox) ispravno centrira oko stvarnog sadržaja
        UpdateOptionsBoxHeight(dialogueOptions.Length);

        float elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            await YarnTask.Yield();
        }

        foreach (var btn in spawnedButtons)
        {
            var b = btn.GetComponentInChildren<Button>();
            if (b != null) b.interactable = true;
        }

        while (selectedIndex == -1 && !cancellationToken.NextContentToken.IsCancellationRequested)
        {
            await YarnTask.Yield();
        }

        if (selectedIndex == -1) { ClearButtons(); return null; }

        var selected = dialogueOptions[selectedIndex];
        ClearButtons();
        return selected;
    }

    // NOVO — postavlja preferredHeight na OptionsBox, pa Content Size Fitter/Layout Group
    // na DialogueContaineru automatski preračuna centriranje, bez ručnog anchoredPosition računanja
    private void UpdateOptionsBoxHeight(int optionCount)
    {
        if (optionsBoxLayoutElement != null)
        {
            optionsBoxLayoutElement.preferredHeight = optionCount > 0
                ? optionCount * buttonHeight + (optionCount - 1) * buttonSpacing
                : 0f;
        }

        if (dialogueContainerRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueContainerRect);
        }
    }

    private void ClearButtons()
    {
        foreach (var btn in spawnedButtons)
        {
            if (btn != null) Destroy(btn);
        }
        spawnedButtons.Clear();
        selectedIndex = -1;

        // NOVO — kad nema opcija (linija bez izbora), OptionsBox se stisne na 0
        // pa se DialogueBox centrira sam, bez praznog prostora ispod
        UpdateOptionsBoxHeight(0);
    }
}