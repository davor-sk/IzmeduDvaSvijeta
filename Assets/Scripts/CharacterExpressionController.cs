using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class CharacterExpressionController : MonoBehaviour
{
    [System.Serializable]
    public class ExpressionSet
    {
        public string characterName;

        public Sprite neutral;
        public Sprite happy;
        public Sprite angry;
        public Sprite doubtful;
        public Sprite sad;
        public Sprite surprised;
    }

    [Header("UI")]
    [SerializeField] private Image avatarImage;

    [Header("Expressions")]
    [SerializeField] private List<ExpressionSet> characters = new List<ExpressionSet>();

    private Dictionary<string, Sprite> currentExpressions =
        new Dictionary<string, Sprite>();

    [YarnCommand("expression")]
    public void SetExpression(string characterName, string expression)
    {
        ExpressionSet character = characters.Find(
            c => c.characterName.ToLower() == characterName.ToLower()
        );

        if (character == null)
        {
            Debug.LogWarning($"Character '{characterName}' not found.");
            return;
        }

        Sprite sprite = GetExpressionSprite(character, expression);

        if (sprite == null)
        {
            Debug.LogWarning(
                $"Expression '{expression}' not found for character '{characterName}'."
            );
            return;
        }

        currentExpressions[characterName.ToLower()] = sprite;

        // Odmah promijeni sliku ako je taj lik trenutno prikazan.
        avatarImage.sprite = sprite;
    }

    public Sprite GetCurrentExpression(string characterName)
    {
        if (string.IsNullOrEmpty(characterName))
            return null;

        currentExpressions.TryGetValue(
            characterName.ToLower(),
            out Sprite sprite
        );

        return sprite;
    }

    private Sprite GetExpressionSprite(
        ExpressionSet character,
        string expression)
    {
        switch (expression.ToLower())
        {
            case "neutral":
                return character.neutral;

            case "happy":
                return character.happy;

            case "angry":
                return character.angry;

            case "doubtful":
                return character.doubtful;

            case "sad":
                return character.sad;

            case "surprised":
                return character.surprised;

            default:
                return character.neutral;
        }
    }
}