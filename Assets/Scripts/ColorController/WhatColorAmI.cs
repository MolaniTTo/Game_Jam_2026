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
        if (isPainted) return;

        // Comprova que el color aplicat sigui l'actiu a VisualDracColor
        VisualDracColor visual = VisualDracColor.Instance;
        if (visual != null && visual.ColorActiu != appliedColor)
        {
            // Color no és l'actiu ara mateix — feedback visual d'error i torna a blanc
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            changeColor.ChangeColorSculture(appliedColor.color);
            fadeCoroutine = StartCoroutine(FadeToWhite(appliedColor.color));
            return;
        }

        if (appliedColor == validColor)
        {
            isPainted = true;
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            changeColor.ChangeColorSculture(validColor.color);
            RoundController.Instance.PaintGroup(this, validColor);
        }
        else
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            changeColor.ChangeColorSculture(appliedColor.color);
            fadeCoroutine = StartCoroutine(FadeToWhite(appliedColor.color));
        }
    }

    public void PaintDirect(ColorSO color)
    {
        if (isPainted) return;
        isPainted = true;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        changeColor.ChangeColorSculture(color.color);
    }

    public void ResetPece()
    {
        isPainted = false;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        changeColor.ChangeColorSculture(Color.white);
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
    public ColorSO GetValidColor() => validColor;
}
