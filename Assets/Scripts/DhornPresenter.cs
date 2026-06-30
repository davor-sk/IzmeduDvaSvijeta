using System.Threading;
using UnityEngine;
using TMPro;
using Yarn.Unity;

public class DhornPresenter : DialoguePresenterBase
{
    public TextMeshProUGUI messageText;

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
        messageText.text = line.TextWithoutCharacterName.Text;
        await YarnTask.WaitUntilCanceled(token.NextContentToken);
    }

    [System.Obsolete]
    public override YarnTask<DialogueOption> RunOptionsAsync(DialogueOption[] dialogueOptions, CancellationToken cancellationToken)
    {
        return YarnTask<DialogueOption>.FromResult(null);
    }
}