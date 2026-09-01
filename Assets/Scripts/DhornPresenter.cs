using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Yarn.Unity;
using UnityEngine.UI;

public class DhornPresenter : DialoguePresenterBase
{
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Image avatarImage;
    public Button nextButton;
    public CharacterExpressionController expressionController;

    [Header("Auto Advance")]
    public Button autoAdvanceToggle;
    public TextMeshProUGUI autoAdvanceToggleText;
    public float autoAdvanceDelay = 3.5f;

    private static bool autoAdvanceEnabled = false;

    [Header("Typewriter")]
    [Min(1f)]
    public float charactersPerSecond = 40f;

    [System.Serializable]
    public class SpeakerVisual
    {
        public string speakerName;
        public Color32 speakerNameColor;
        public Color32 dialogueColor;
        public TMP_FontAsset dialogueFont;
        public TMP_FontAsset speakerNameFont;
        public Sprite avatarSprite;
        public bool avatarOnRight;
    }

    public List<SpeakerVisual> speakerVisuals;

    private static readonly Color32 DefaultColor =
        new Color32(224, 224, 224, 255);

    public override YarnTask OnDialogueStartedAsync()
    {
        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        return YarnTask.CompletedTask;
    }

    void Start()
    {
        if (autoAdvanceToggle != null)
        {
            autoAdvanceToggle.onClick.AddListener(ToggleAutoAdvance);
            UpdateToggleVisual();
        }
    }

    void ToggleAutoAdvance()
    {
        autoAdvanceEnabled = !autoAdvanceEnabled;
        UpdateToggleVisual();
    }

    void UpdateToggleVisual()
    {
        if (autoAdvanceToggleText != null)
        {
            autoAdvanceToggleText.text = autoAdvanceEnabled ? "AUTO: ON" : "AUTO: OFF";
            autoAdvanceToggleText.color = autoAdvanceEnabled
                ? new Color32(76, 175, 80, 255)
                : new Color32(180, 180, 180, 255);
        }
    }

    public override async YarnTask RunLineAsync(
        LocalizedLine line,
        LineCancellationToken token)
    {
        string speaker = line.CharacterName ?? "";
        string text = line.TextWithoutCharacterName.Text;

        speakerNameText.text = speaker;

        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        var visual = speakerVisuals?.Find(
            v => v.speakerName == speaker
        );

        if (visual != null)
        {
            speakerNameText.color = visual.speakerNameColor;
            dialogueText.color = visual.dialogueColor;

            if (visual.dialogueFont != null)
                dialogueText.font = visual.dialogueFont;

            if (visual.speakerNameFont != null)
                speakerNameText.font = visual.speakerNameFont;

            Sprite expressionSprite = null;

            if (expressionController != null)
            {
                expressionSprite =
                    expressionController.GetCurrentExpression(speaker);
            }

            Sprite spriteToUse =
                expressionSprite != null
                    ? expressionSprite
                    : visual.avatarSprite;

            if (spriteToUse != null)
            {
                avatarImage.gameObject.SetActive(true);
                avatarImage.sprite = spriteToUse;

                if (visual.avatarOnRight)
                    avatarImage.transform.SetAsLastSibling();
                else
                    avatarImage.transform.SetAsFirstSibling();
            }
            else
            {
                avatarImage.gameObject.SetActive(false);
            }
        }
        else
        {
            speakerNameText.color = DefaultColor;
            dialogueText.color = DefaultColor;
            avatarImage.gameObject.SetActive(false);
        }

        dialogueText.ForceMeshUpdate();

        int totalCharacters = dialogueText.textInfo.characterCount;

        bool isTyping = true;
        bool skipTypewriter = false;
        bool advanced = false;
        bool listenerAdded = false;

        void OnNextClicked()
        {
            if (isTyping)
            {
                skipTypewriter = true;
            }
            else
            {
                advanced = true;
            }
        }

        // NOVO — pomoćne funkcije koje garantiraju točno stanje gumba
        void EnsureButtonShown()
        {
            if (!listenerAdded)
            {
                nextButton.onClick.AddListener(OnNextClicked);
                listenerAdded = true;
            }
            nextButton.gameObject.SetActive(true);
        }

        void EnsureButtonHidden()
        {
            if (listenerAdded)
            {
                nextButton.onClick.RemoveListener(OnNextClicked);
                listenerAdded = false;
            }
            nextButton.gameObject.SetActive(false);
        }

        if (autoAdvanceEnabled)
            EnsureButtonHidden();
        else
            EnsureButtonShown();

        float visibleCharacters = 0f;

        // =========================
        // TYPEWRITER
        // =========================

        while (dialogueText.maxVisibleCharacters < totalCharacters)
        {
            if (skipTypewriter)
            {
                dialogueText.maxVisibleCharacters = totalCharacters;
                break;
            }

            if (Keyboard.current != null &&
                (Keyboard.current.enterKey.wasPressedThisFrame ||
                 Keyboard.current.spaceKey.wasPressedThisFrame))
            {
                dialogueText.maxVisibleCharacters = totalCharacters;
                break;
            }

            visibleCharacters +=
                charactersPerSecond * Time.deltaTime;

            dialogueText.maxVisibleCharacters =
                Mathf.Min(
                    Mathf.FloorToInt(visibleCharacters),
                    totalCharacters
                );

            await YarnTask.Yield();
        }

        dialogueText.maxVisibleCharacters = totalCharacters;

        isTyping = false;

        await YarnTask.Yield();

        // =========================
        // ČEKANJE NA "DALJE" (ili auto-advance)
        // =========================

        // NOVO — ponovno provjeri stanje OVDJE, jer se moglo promijeniti
        // dok je typewriter bio u tijeku (uključen ili isključen usred linije).
        if (autoAdvanceEnabled)
        {
            EnsureButtonHidden();

            float waitElapsed = 0f;

            while (!advanced)
            {
                if (Keyboard.current != null &&
                    (Keyboard.current.enterKey.wasPressedThisFrame ||
                     Keyboard.current.spaceKey.wasPressedThisFrame))
                {
                    advanced = true;
                }
                else if (waitElapsed >= autoAdvanceDelay)
                {
                    advanced = true;
                }

                waitElapsed += Time.deltaTime;
                await YarnTask.Yield();
            }
        }
        else
        {
            EnsureButtonShown();

            while (!advanced)
            {
                if (Keyboard.current != null &&
                    (Keyboard.current.enterKey.wasPressedThisFrame ||
                     Keyboard.current.spaceKey.wasPressedThisFrame))
                {
                    advanced = true;
                }

                await YarnTask.Yield();
            }

            EnsureButtonHidden();
        }
    }

    [System.Obsolete]
    public override YarnTask<DialogueOption> RunOptionsAsync(
        DialogueOption[] dialogueOptions,
        CancellationToken cancellationToken)
    {
        return YarnTask<DialogueOption>.FromResult(null);
    }
}