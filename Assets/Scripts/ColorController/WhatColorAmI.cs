using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhatColorAmI : MonoBehaviour
{
    [Header("Color vàlid per aquesta peça")]
    [SerializeField] private ColorSO validColor;

    [Header("Configuració del fade")]
    [SerializeField] private float fadeDelay = 0.5f;    // temps que es queda amb el color incorrecte
    [SerializeField] private float fadeDuration = 1.5f; // temps que tarda a tornar a blanc

    private ChangeColor changeColor;
    private Coroutine fadeCoroutine;
    private bool isPainted = false; // true quan té el color correcte

    void Awake()
    {
        changeColor = GetComponent<ChangeColor>();
    }

    // Cridat des de PlayerController en lloc de changeColor.ChangeColorSculture directament
    public void TryPaint(ColorSO appliedColor)
    {
        if (isPainted) return; // ja té el color correcte, no es pot repintar

        if (appliedColor == validColor)
        {
            // Color correcte — es queda
            isPainted = true;
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            changeColor.ChangeColorSculture(validColor.color);
        }
        else
        {
            // Color incorrecte — es pinta i torna a blanc
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            changeColor.ChangeColorSculture(appliedColor.color);
            fadeCoroutine = StartCoroutine(FadeToWhite(appliedColor.color));
        }
    }

    private IEnumerator FadeToWhite(Color fromColor)
    {
        yield return new WaitForSeconds(fadeDelay);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            Color current = Color.Lerp(fromColor, Color.white, elapsed / fadeDuration);
            changeColor.ChangeColorSculture(current);
            yield return null;
        }

        changeColor.ChangeColorSculture(Color.white);
    }

    public bool IsPainted => isPainted;

    public void SetValidColor(ColorSO color)
    {
        validColor = color;
    }
}