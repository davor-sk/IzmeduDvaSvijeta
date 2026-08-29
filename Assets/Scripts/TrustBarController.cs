using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class TrustBarController : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public RectTransform dhornFill;
    public RectTransform governmentFill;
    public TextMeshProUGUI dhornLabel;
    public TextMeshProUGUI governmentLabel;

    public float maxBarHalfWidth = 200f;
    public float displayMinValue = 20f;   // NOVO — donja granica raspona koji se prikazuje
    public float displayMaxValue = 80f;   // NOVO — gornja granica raspona koji se prikazuje
    public float smoothSpeed = 4f;        // NOVO — brzina animiranog prijelaza

    private float currentDhornWidth;
    private float currentGovernmentWidth;

    void Update()
    {
        if (dialogueRunner == null || dialogueRunner.VariableStorage == null) return;

        float dhornTrust = GetVariable("$dhornTrust", 50f);
        float governmentTrust = GetVariable("$governmentTrust", 50f);

        float targetDhornWidth = Normalize(dhornTrust) * maxBarHalfWidth;
        float targetGovernmentWidth = Normalize(governmentTrust) * maxBarHalfWidth;

        currentDhornWidth = Mathf.Lerp(currentDhornWidth, targetDhornWidth, Time.deltaTime * smoothSpeed);
        currentGovernmentWidth = Mathf.Lerp(currentGovernmentWidth, targetGovernmentWidth, Time.deltaTime * smoothSpeed);

        dhornFill.sizeDelta = new Vector2(currentDhornWidth, dhornFill.sizeDelta.y);
        governmentFill.sizeDelta = new Vector2(currentGovernmentWidth, governmentFill.sizeDelta.y);

        if (dhornLabel != null) dhornLabel.text = $"DHORN {Mathf.RoundToInt(dhornTrust)}";
        if (governmentLabel != null) governmentLabel.text = $"VLADA {Mathf.RoundToInt(governmentTrust)}";
    }

    float Normalize(float value)
    {
        return Mathf.Clamp01((value - displayMinValue) / (displayMaxValue - displayMinValue));
    }

    float GetVariable(string name, float fallback)
    {
        if (dialogueRunner.VariableStorage.TryGetValue<float>(name, out float value))
        {
            return value;
        }
        return fallback;
    }
}