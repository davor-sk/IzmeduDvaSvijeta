using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Yarn.Unity;

public class DhornPresenter : DialoguePresenterBase
{
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

    [System.Serializable]
    public class SpeakerVisual
    {
        public string speakerName;
        public Color32 speakerNameColor;
        public Color32 dialogueColor;
        public TMP_FontAsset dialogueFont;
        public TMP_FontAsset speakerNameFont;
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

        var visual = speakerVisuals?.Find(v => v.speakerName == speaker);

        if (visual != null)
        {
            speakerNameText.color = visual.speakerNameColor;
            dialogueText.color = visual.dialogueColor;
            if (visual.dialogueFont != null) dialogueText.font = visual.dialogueFont;
            if (visual.speakerNameFont != null) speakerNameText.font = visual.speakerNameFont;
        }
        else
        {
            speakerNameText.color = DefaultColor;
            dialogueText.color = DefaultColor;
        }

        float autoAdvanceDelay = 3.5f;
        float elapsed = 0f;
        bool advanced = false;

        while (!advanced)
        {
            if (Keyboard.current != null && (
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame))
            {
                advanced = true;
            }
            else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                advanced = true;
            }
            else if (elapsed >= autoAdvanceDelay)
            {
                advanced = true;
            }

            elapsed += Time.deltaTime;
            await YarnTask.Yield();
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