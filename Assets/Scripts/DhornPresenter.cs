using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class DhornPresenter : DialoguePresenterBase
{
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Image avatarImage;
    public Button nextButton;

    public float typewriterSpeed = 40f; // znakova po sekundi

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

    private static readonly Color32 DefaultColor = new Color32(224, 224, 224, 255);

    public override YarnTask OnDialogueStartedAsync()
    {
        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        return YarnTask.CompletedTask;
    }

    public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        string speaker = line.CharacterName ?? "";
        string text = line.TextWithoutCharacterName.Text;

        speakerNameText.text = speaker;
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        var visual = speakerVisuals?.Find(v => v.speakerName == speaker);

        if (visual != null)
        {
            speakerNameText.color = visual.speakerNameColor;
            dialogueText.color = visual.dialogueColor;
            if (visual.dialogueFont != null) dialogueText.font = visual.dialogueFont;
            if (visual.speakerNameFont != null) speakerNameText.font = visual.speakerNameFont;

            if (visual.avatarSprite != null)
            {
                avatarImage.gameObject.SetActive(true);
                avatarImage.sprite = visual.avatarSprite;
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

        nextButton.gameObject.SetActive(false);

        // Typewriter efekt
        int totalChars = text.Length;
        float elapsed = 0f;
        bool skipRequested = false;

        while (dialogueText.maxVisibleCharacters < totalChars)
        {
            if (Keyboard.current != null && (
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame))
            {
                skipRequested = true;
            }

            if (skipRequested)
            {
                dialogueText.maxVisibleCharacters = totalChars;
                break;
            }

            elapsed += Time.deltaTime * typewriterSpeed;
            dialogueText.maxVisibleCharacters = Mathf.FloorToInt(elapsed);
            await YarnTask.Yield();
        }

        dialogueText.maxVisibleCharacters = totalChars;

        // Čekanje na napredovanje (NextButton / Enter / Space)
        bool advanced = false;
        void OnNextClicked() => advanced = true;

        nextButton.onClick.AddListener(OnNextClicked);
        nextButton.gameObject.SetActive(true);

        while (!advanced)
        {
            if (Keyboard.current != null && (
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame))
            {
                advanced = true;
            }

            await YarnTask.Yield();
        }

        nextButton.onClick.RemoveListener(OnNextClicked);
        nextButton.gameObject.SetActive(false);
    }

    [System.Obsolete]
    public override YarnTask<DialogueOption> RunOptionsAsync(
        DialogueOption[] dialogueOptions,
        CancellationToken cancellationToken)
    {
        return YarnTask<DialogueOption>.FromResult(null);
    }
}