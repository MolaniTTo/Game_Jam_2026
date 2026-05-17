using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ContornColorController : MonoBehaviour
{
    private Image imagenUI;
    private Coroutine parpadeoCoroutine;

    private void Awake()
    {
        imagenUI = GetComponent<Image>();
        SetAlpha(0f); // invisible al inici
    }

    public void IniciarParpadeo(Color color, float durada, float interval = 0.2f)
    {
        if (parpadeoCoroutine != null) StopCoroutine(parpadeoCoroutine);
        parpadeoCoroutine = StartCoroutine(ParpadeoRoutine(color, durada, interval));
    }

    public void Amagar()
    {
        if (parpadeoCoroutine != null)
        {
            StopCoroutine(parpadeoCoroutine);
            parpadeoCoroutine = null;
        }
        SetAlpha(0f);
    }

    private IEnumerator ParpadeoRoutine(Color color, float durada, float interval)
    {
        // FadeIn ràpid
        imagenUI.color = color;
        float fadeElapsed = 0f;
        float fadeDuration = 0.15f;
        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0f, 1f, fadeElapsed / fadeDuration));
            yield return null;
        }
        SetAlpha(1f);

        // Parpadeo durant 'durada' segons
        float elapsed = 0f;
        bool visible = true;
        while (elapsed < durada)
        {
            visible = !visible;
            SetAlpha(visible ? 1f : 0f);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color c = imagenUI.color;
        c.a = alpha;
        imagenUI.color = c;
    }
}