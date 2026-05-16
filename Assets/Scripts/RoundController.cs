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
        if (palettes.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, palettes.Count);

        currentPalette = palettes[randomIndex];

        colorsResultat = currentPalette.colors;

        return currentPalette.colors;
    }
}