using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDracColor : MonoBehaviour
{
    [Header("Llum de color actiu")]
    [SerializeField] private Light colorLight;

    [Header("Configuració de temps")]
    [SerializeField] private float tempsParpadeixRapid = 3f;
    [SerializeField] private float velocityLerp = 1f;

    private List<ColorSO> paleta;
    private List<WhatColorAmI> totesPeces;
    private int indexColorActiu = -1;
    private Coroutine cicleCoroutine;

    private void Start()
    {
        if (colorLight != null) colorLight.enabled = false;
    }

    // Cridat pel RoundManager quan acaba el tutorial
    public void IniciarCicle()
    {
        if (cicleCoroutine != null) StopCoroutine(cicleCoroutine);

        // Agafa la paleta i peces actualitzades del RoundController
        paleta = new List<ColorSO>(RoundController.Instance.colorsResultat);
        totesPeces = RoundController.Instance.whatColorAmI;

        if (colorLight != null) colorLight.enabled = false;
        cicleCoroutine = StartCoroutine(CicleColors());
    }

    // Cridat pel RoundManager quan acaba la ronda
    public void AturaciCicle()
    {
        if (cicleCoroutine != null)
        {
            StopCoroutine(cicleCoroutine);
            cicleCoroutine = null;
        }
        if (colorLight != null) colorLight.enabled = false;

        // Reset visual de totes les peces
        if (totesPeces != null)
            foreach (var peca in totesPeces)
                if (!peca.IsPainted)
                    peca.GetComponent<ChangeColor>().ChangeColorSculture(Color.white);
    }

    private IEnumerator CicleColors()
    {
        int ultimIndex = -1;

        while (true)
        {
            int nouIndex;
            do { nouIndex = Random.Range(0, paleta.Count); }
            while (nouIndex == ultimIndex && paleta.Count > 1);

            ultimIndex = nouIndex;
            indexColorActiu = nouIndex;
            ColorSO colorActiu = paleta[indexColorActiu];

            RoundController.Instance.NotificarColorActiu(colorActiu);

            float tempsTotal = RoundController.Instance.tempsPerColor;
            float tempsNormal = tempsTotal - tempsParpadeixRapid;

            if (colorLight != null)
            {
                colorLight.enabled = true;
                colorLight.color = colorActiu.color;
            }

            yield return StartCoroutine(FaseNormal(colorActiu, tempsNormal));
            yield return StartCoroutine(FaseParpadeix(colorActiu, tempsParpadeixRapid));

            if (colorLight != null) colorLight.enabled = false;
            ResetPecesColor(colorActiu);

            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator FaseNormal(ColorSO colorActiu, float durada)
    {
        float elapsed = 0f;
        while (elapsed < 1f / velocityLerp)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed * velocityLerp);
            SetColorPeces(colorActiu, Color.Lerp(Color.white, colorActiu.color, t));
            yield return null;
        }
        SetColorPeces(colorActiu, colorActiu.color);
        float tempsRestant = durada - (1f / velocityLerp);
        if (tempsRestant > 0f)
            yield return new WaitForSeconds(tempsRestant);
    }

    private IEnumerator FaseParpadeix(ColorSO colorActiu, float durada)
    {
        float elapsed = 0f;
        float intervalRapid = 0.2f;
        bool visible = true;

        while (elapsed < durada)
        {
            visible = !visible;
            SetColorPeces(colorActiu, visible ? colorActiu.color : Color.white);
            if (colorLight != null) colorLight.enabled = visible;
            yield return new WaitForSeconds(intervalRapid);
            elapsed += intervalRapid;
        }
    }

    private void SetColorPeces(ColorSO colorActiu, Color color)
    {
        if (totesPeces == null) return;
        foreach (WhatColorAmI peca in totesPeces)
            if (!peca.IsPainted && peca.GetValidColor() == colorActiu)
                peca.GetComponent<ChangeColor>().ChangeColorSculture(color);
    }

    private void ResetPecesColor(ColorSO colorActiu)
    {
        if (totesPeces == null) return;
        foreach (WhatColorAmI peca in totesPeces)
            if (!peca.IsPainted && peca.GetValidColor() == colorActiu)
                peca.GetComponent<ChangeColor>().ChangeColorSculture(Color.white);
    }
}