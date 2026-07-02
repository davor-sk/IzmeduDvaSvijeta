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
    
    while (true)
    {
        bool advance =
            (Keyboard.current != null &&
                (Keyboard.current.enterKey.wasPressedThisFrame ||
                 Keyboard.current.spaceKey.wasPressedThisFrame))
            ||
            (Mouse.current != null &&
                Mouse.current.leftButton.wasPressedThisFrame);

        if (advance)
            break;

        await YarnTask.Yield();
    }
    
    await YarnTask.Yield();
}
}