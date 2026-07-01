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
        speakerNameText.text = line.CharacterName ?? "";
        dialogueText.text = line.TextWithoutCharacterName.Text;

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