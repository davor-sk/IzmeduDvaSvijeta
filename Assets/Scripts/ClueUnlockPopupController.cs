using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClueUnlockPopupController : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI clueUnlockText;
    public float fadeDuration = 0.4f;
    public float displayDuration = 3f;

    private Queue<string> queue = new Queue<string>();
    private bool isShowing = false;

    void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void Enqueue(string text)
    {
        queue.Enqueue(text);
        if (!isShowing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    IEnumerator ProcessQueue()
    {
        isShowing = true;
        while (queue.Count > 0)
        {
            string text = queue.Dequeue();
            clueUnlockText.text = text;

            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
        }
        isShowing = false;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}