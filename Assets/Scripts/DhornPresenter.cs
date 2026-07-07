using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Yarn.Unity;

public class DhornPresenter : DialoguePresenterBase
{
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

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

        switch (speaker)
        {
            case "Dhorn":
                speakerNameText.color = new Color32(76, 175, 80, 255);
                dialogueText.color = new Color32(200, 230, 200, 255);
                break;
            case "Kael":
                speakerNameText.color = new Color32(224, 224, 224, 255);
                dialogueText.color = new Color32(224, 224, 224, 255);
                break;
            case "Maren":
                speakerNameText.color = new Color32(255, 152, 0, 255);
                dialogueText.color = new Color32(255, 220, 180, 255);
                break;
            case "Narrator":
                speakerNameText.color = new Color32(158, 158, 158, 255);
                dialogueText.color = new Color32(158, 158, 158, 255);
                break;
            default:
                speakerNameText.color = new Color32(224, 224, 224, 255);
                dialogueText.color = new Color32(224, 224, 224, 255);
                break;
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