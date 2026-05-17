using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressColorHUD : MonoBehaviour
{
    [Header("Imatges en ordre (índex 0 = primer color, etc.)")]
    [SerializeField] private List<Image> colorImages;

    // colorSO assignat a cada imatge (en el mateix ordre)
    private List<ColorSO> colorsAssignats = new List<ColorSO>();
    // total de peces per color
    private Dictionary<ColorSO, int> totalPecesPerColor = new Dictionary<ColorSO, int>();
    // peces pintades per color
    private Dictionary<ColorSO, int> pintadesPerColor = new Dictionary<ColorSO, int>();

    // Cridat pel RoundManager quan s'activa el prefab
    public void Inicialitzar(List<ColorSO> paleta, List<WhatColorAmI> totesPeces)
    {
        colorsAssignats = new List<ColorSO>(paleta);
        totalPecesPerColor.Clear();
        pintadesPerColor.Clear();

        // Compta quantes peces hi ha de cada color
        foreach (var color in paleta)
        {
            totalPecesPerColor[color] = 0;
            pintadesPerColor[color] = 0;
        }
        foreach (var peca in totesPeces)
        {
            ColorSO c = peca.GetValidColor();
            if (c != null && totalPecesPerColor.ContainsKey(c))
                totalPecesPerColor[c]++;
        }

        // Configura les imatges: color i fill a 0
        for (int i = 0; i < colorImages.Count; i++)
        {
            if (i < colorsAssignats.Count)
            {
                colorImages[i].gameObject.SetActive(true);
                colorImages[i].color = colorsAssignats[i].color;
                colorImages[i].fillAmount = 0f;
            }
            else
            {
                colorImages[i].gameObject.SetActive(false);
            }
        }
    }

    // Cridat quan es pinta una peça
    public void NotificarPecaPintada(ColorSO color)
    {
        if (!pintadesPerColor.ContainsKey(color)) return;

        pintadesPerColor[color]++;

        int idx = colorsAssignats.IndexOf(color);
        if (idx < 0 || idx >= colorImages.Count) return;

        int total = totalPecesPerColor[color];
        if (total == 0) return;

        colorImages[idx].fillAmount = (float)pintadesPerColor[color] / total;
    }

    // Cridat al reset de ronda
    public void Reset()
    {
        foreach (var img in colorImages)
        {
            img.fillAmount = 0f;
            img.color = Color.white;
        }
        colorsAssignats.Clear();
        totalPecesPerColor.Clear();
        pintadesPerColor.Clear();
    }
}