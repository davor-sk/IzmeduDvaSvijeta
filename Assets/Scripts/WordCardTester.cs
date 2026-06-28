using UnityEngine;

public class WordCardTester : MonoBehaviour
{
    public WordCardDisplay card;

    void Start()
    {
        card.SetState(
            WordCardDisplay.WordState.IZBOR,
            "moran",
            new string[] { "doma", "sigurnosti", "nade" }
        );
    }
}