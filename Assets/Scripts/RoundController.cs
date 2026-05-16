using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    public static RoundController Instance { get; private set; }

    [Header("Todas las paletas posibles")]
    [SerializeField] private List<PaletteSO> palettes;

    [Header("Paleta actual")]
    [SerializeField] private PaletteSO currentPalette;

    public List<ColorSO> colorsResultat;

    public List<WhatColorAmI> whatColorAmI;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        GenerarPaleta();
    }

    public List<ColorSO> GenerarPaleta()
    {
        if (palettes.Count == 0) return null;
        int randomIndex = Random.Range(0, palettes.Count);
        currentPalette = palettes[randomIndex];
        colorsResultat = currentPalette.colors;
        AssignarColorsPeces(); 
        return currentPalette.colors;
    }

    public void AssignarColorsPeces()
    {
        if (colorsResultat == null || colorsResultat.Count == 0 || whatColorAmI == null || whatColorAmI.Count == 0) return;

        // Crea una llista amb els colors repartits equilibradament
        List<ColorSO> colorsAssignats = new List<ColorSO>();
        int numColors = colorsResultat.Count;
        int numPeces = whatColorAmI.Count;

        // Omple la llista repetint els colors fins a cobrir totes les peces
        for (int i = 0; i < numPeces; i++)
            colorsAssignats.Add(colorsResultat[i % numColors]);

        // Barreja la llista per que no quedi en ordre
        for (int i = colorsAssignats.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (colorsAssignats[i], colorsAssignats[j]) = (colorsAssignats[j], colorsAssignats[i]);
        }

        // Assigna cada color a cada peça
        for (int i = 0; i < numPeces; i++)
            whatColorAmI[i].SetValidColor(colorsAssignats[i]);
    }

}