using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDracColor : MonoBehaviour
{
    public static VisualDracColor Instance { get; private set; }

    [Header("Llum de color actiu")]
    [SerializeField] private Light colorLight;
    [SerializeField] private Light colorLight1;

    [Header("Configuració de temps")]
    [SerializeField] private float tempsPerColor = 10f;
    [SerializeField] private float intervalInici = 1.5f;   // interval de parpadeo al principi (lent)
    [SerializeField] private float intervalFinal = 0.1f;   // interval de parpadeo al final (ràpid)

    [Header("UI Contorn")]
    [SerializeField] private ContornColorController contornColor;

    [Header("Visual ajuda")]
    [SerializeField, Range(0f, 1f)] private float alphaAjuda = 0.3f;

    private HashSet<ColorSO> colorsCompletats = new HashSet<ColorSO>();
    public ColorSO ColorActiu { get; private set; }

    private List<ColorSO> paleta;
    private List<WhatColorAmI> totesPeces;
    private int indexColorActiu = -1;
    private Coroutine cicleCoroutine;

    public float TempsRestantColor { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (colorLight != null) colorLight.enabled = false;
        if (colorLight1 != null) colorLight1.enabled = false;
    }

    public void MarcarColorComplet(ColorSO color)
    {
        colorsCompletats.Add(color);
    }

    private Color ColorMesClar(Color color, float quantitat)
    {
        return Color.Lerp(color, Color.white, quantitat);
    }

    public void IniciarCicle()
    {
        if (cicleCoroutine != null) StopCoroutine(cicleCoroutine);
        colorsCompletats.Clear();
        paleta = new List<ColorSO>(RoundController.Instance.colorsResultat);
        totesPeces = RoundController.Instance.whatColorAmI;
        if (colorLight != null) colorLight.enabled = false;
        if (colorLight1 != null) colorLight1.enabled = false;
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
        if (colorLight1 != null) colorLight1.enabled = false;
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
            // Filtra colors no completats
            List<int> indexosDisponibles = new List<int>();
            for (int i = 0; i < paleta.Count; i++)
                if (!colorsCompletats.Contains(paleta[i]))
                    indexosDisponibles.Add(i);

            if (indexosDisponibles.Count == 0)
            {
                if (colorLight != null) colorLight.enabled = false;
                if (colorLight1 != null) colorLight1.enabled = false;
                if (contornColor != null) contornColor.Amagar();
                yield break;
            }

            // Escull color
            int nouIndex;
            if (indexosDisponibles.Count == 1)
            {
                nouIndex = indexosDisponibles[0];
            }
            else
            {
                int pick;
                do { pick = Random.Range(0, indexosDisponibles.Count); }
                while (paleta.IndexOf(paleta[indexosDisponibles[pick]]) == ultimIndex);
                nouIndex = indexosDisponibles[pick];
            }

            ultimIndex = nouIndex;
            indexColorActiu = nouIndex;
            ColorActiu = paleta[indexColorActiu];

            RoundController.Instance.NotificarColorActiu(ColorActiu);

            // Activa llums i contorn
            SetLlums(true, ColorActiu.color);
            if (contornColor != null)
                contornColor.IniciarParpadeo(ColorActiu.color, tempsPerColor);

            TempsRestantColor = tempsPerColor;
            // Parpadeo progressiu durant tot el temps del color
            yield return StartCoroutine(FaseParpadeoProgressiu(ColorActiu));

            // Neteja
            SetLlums(false, Color.white);
            if (contornColor != null) contornColor.Amagar();
            ResetPecesColor(ColorActiu);

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator FaseParpadeoProgressiu(ColorSO colorActiu)
    {
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < tempsPerColor)
        {
            TempsRestantColor = tempsPerColor - elapsed; // <-- afegeix això

            float t = elapsed / tempsPerColor;
            float intervalActual = Mathf.Lerp(intervalInici, intervalFinal, t);

            visible = !visible;
            Color colorVisual = visible ? ColorMesClar(colorActiu.color, alphaAjuda) : Color.white;
            SetColorPeces(colorActiu, colorVisual);
            SetLlums(visible, colorActiu.color);

            yield return new WaitForSeconds(intervalActual);
            elapsed += intervalActual;
        }

        TempsRestantColor = 0f;
        SetColorPeces(colorActiu, ColorMesClar(colorActiu.color, alphaAjuda));
        SetLlums(true, colorActiu.color);
    }

    private void SetLlums(bool actives, Color color)
    {
        if (colorLight != null)
        {
            colorLight.enabled = actives;
            if (actives) colorLight.color = color;
        }
        if (colorLight1 != null)
        {
            colorLight1.enabled = actives;
            if (actives) colorLight1.color = color;
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