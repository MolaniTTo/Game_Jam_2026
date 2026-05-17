using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDracColor : MonoBehaviour
{
    public static VisualDracColor Instance { get; private set; }

    [Header("Llum de color actiu")]
    [SerializeField] private Light colorLight;

    [Header("Configuració de temps")]
    [SerializeField] private float tempsParpadeixRapid = 5f; // canviat a 5 segons
    [SerializeField] private float velocityLerp = 1f;

    [Header("UI Contorn")]
    [SerializeField] private ContornColorController contornColor;
    public ColorSO ColorActiu { get; private set; }

    private List<ColorSO> paleta;
    private List<WhatColorAmI> totesPeces;
    private int indexColorActiu = -1;
    private Coroutine cicleCoroutine;

    

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (colorLight != null) colorLight.enabled = false;
    }

    public void IniciarCicle()
    {
        if (cicleCoroutine != null) StopCoroutine(cicleCoroutine);
        paleta = new List<ColorSO>(RoundController.Instance.colorsResultat);
        totesPeces = RoundController.Instance.whatColorAmI;
        if (colorLight != null) colorLight.enabled = false;
        cicleCoroutine = StartCoroutine(CicleColors());
    }

    public void AturaciCicle()
    {
        ColorActiu = null;
        if (cicleCoroutine != null)
        {
            StopCoroutine(cicleCoroutine);
            cicleCoroutine = null;
        }
        if (colorLight != null) colorLight.enabled = false;
        if (contornColor != null) contornColor.Amagar();

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
            ColorActiu = paleta[indexColorActiu];

            RoundController.Instance.NotificarColorActiu(ColorActiu);

            float tempsTotal = RoundController.Instance.tempsPerColor;
            float tempsNormal = tempsTotal - tempsParpadeixRapid;

            if (colorLight != null)
            {
                colorLight.enabled = true;
                colorLight.color = ColorActiu.color;
            }

            // Fase normal — sense parpadeo UI
            yield return StartCoroutine(FaseNormal(ColorActiu, tempsNormal));

            // Fase parpadeo — activa el contorn UI al mateix temps
            if (contornColor != null)
                contornColor.IniciarParpadeo(ColorActiu.color, tempsParpadeixRapid);

            yield return StartCoroutine(FaseParpadeix(ColorActiu, tempsParpadeixRapid));

            if (colorLight != null) colorLight.enabled = false;
            if (contornColor != null) contornColor.Amagar();
            ResetPecesColor(ColorActiu);

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