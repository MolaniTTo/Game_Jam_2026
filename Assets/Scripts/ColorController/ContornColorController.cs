using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ContornColorController : MonoBehaviour
{
    private Image imagenUI;
    private Coroutine coroutine;

    private void Awake()
    {
        imagenUI = GetComponent<Image>();
        SetAlpha(0f);
    }

    public void IniciarParpadeo(Color color, float durada, float interval = 0.2f)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(MostrarColor(color));
    }

    public void Amagar()
    {
        if (coroutine != null) { StopCoroutine(coroutine); coroutine = null; }
        SetAlpha(0f);
    }

    private IEnumerator MostrarColor(Color color)
    {
        imagenUI.color = color;
        float elapsed = 0f;
        float fadeDuration = 0.15f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0f, 1f, elapsed / fadeDuration));
            yield return null;
        }
        SetAlpha(1f);
    }

    private void SetAlpha(float alpha)
    {
        Color c = imagenUI.color;
        c.a = alpha;
        imagenUI.color = c;
    }
}