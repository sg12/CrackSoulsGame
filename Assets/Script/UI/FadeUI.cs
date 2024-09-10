using System.Collections;
using UnityEngine;

public class FadeUI : MonoBehaviour {

    public CanvasGroup canvasGroup;
    [HideInInspector] public bool isFading;

    // Use this for initialization
    void Awake ()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    protected IEnumerator Fade(float finalAlpha, float delay, float duration)
    {
        isFading = true;
        yield return new WaitForSeconds(delay);
        float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / duration;
        while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);
            yield return null;
        }
        canvasGroup.alpha = finalAlpha;
        isFading = false;
    }

    public void FadeTransition(float finalAlpha, float delay, float duration)
    {
        if (!isFading)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(finalAlpha, delay, duration));
            isFading = true;
        }
    }
}
