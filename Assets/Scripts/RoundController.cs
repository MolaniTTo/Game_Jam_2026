using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    public static RoundController Instance { get; private set; }

    [Header("Totes les paletes possibles")]
    [SerializeField] private List<PaletteSO> palettes;

    [Header("Paleta actual")]
    [SerializeField] private PaletteSO currentPalette;

    public List<ColorSO> colorsResultat;

    [Header("Totes les peces de la escena")]
    public List<WhatColorAmI> whatColorAmI;

    [Header("Grups per ronda (índex 0 = ronda 1, índex 1 = ronda 2...)")]
    [SerializeField] private List<ColorGroupsSO> colorGroupsPerRound;

    [Header("Temps per color")]
    [SerializeField] public float tempsPerColor = 10f;

    public event Action<ColorSO> OnColorActiuCanviat;

    private Dictionary<int, List<WhatColorAmI>> groupMap;
    private Dictionary<WhatColorAmI, int> pieceToGroup;

    public int currentRound = 0; // índex de la ronda actual

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        //IniciarRonda(0);
    }

    private void BuildGroupMap()
    {
        groupMap = new Dictionary<int, List<WhatColorAmI>>();
        pieceToGroup = new Dictionary<WhatColorAmI, int>();

        if (colorGroupsPerRound == null || colorGroupsPerRound.Count == 0)
        {
            Debug.LogError("[RoundController] No hi ha ColorGroupsSO assignats!");
            return;
        }

        if (currentRound >= colorGroupsPerRound.Count)
        {
            Debug.LogError($"[RoundController] No hi ha SO per la ronda {currentRound}");
            return;
        }

        ColorGroupsSO currentGroups = colorGroupsPerRound[currentRound];
        var piecesByName = whatColorAmI.ToDictionary(p => p.gameObject.name);

        for (int i = 0; i < currentGroups.groups.Length; i++)
        {
            groupMap[i] = new List<WhatColorAmI>();

            foreach (string pieceName in currentGroups.groups[i].pieceNames)
            {
                if (piecesByName.TryGetValue(pieceName, out var piece))
                {
                    groupMap[i].Add(piece);
                    pieceToGroup[piece] = i;
                }
                else
                {
                    Debug.LogWarning($"[RoundController] Peça '{pieceName}' no trobada (ronda {currentRound})");
                }
            }
        }
    }

    public List<ColorSO> GenerarPaleta()
    {
        if (palettes.Count == 0) return null;
        int randomIndex = UnityEngine.Random.Range(0, palettes.Count);
        currentPalette = palettes[randomIndex];
        colorsResultat = currentPalette.colors;
        AssignarColorsPeces();
        return currentPalette.colors;
    }

    public void AssignarColorsPeces()
    {
        if (colorsResultat == null || colorsResultat.Count == 0 || groupMap == null || groupMap.Count == 0) return;

        List<int> grupIds = new List<int>(groupMap.Keys);
        int numGrups = grupIds.Count;
        int numColors = colorsResultat.Count;

        // Crea una llista que cobreixi tots els grups repartint els colors equilibradament
        List<ColorSO> colorsAssignats = new List<ColorSO>();
        for (int i = 0; i < numGrups; i++)
            colorsAssignats.Add(colorsResultat[i % numColors]);

        // Barreja per que no quedi en ordre
        for (int i = colorsAssignats.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (colorsAssignats[i], colorsAssignats[j]) = (colorsAssignats[j], colorsAssignats[i]);
        }

        // Assigna cada color al seu grup
        for (int i = 0; i < numGrups; i++)
        {
            ColorSO colorAssignat = colorsAssignats[i];
            foreach (var piece in groupMap[grupIds[i]])
                piece.SetValidColor(colorAssignat);
        }

        // Debug per verificar que tots els colors han estat assignats
        Dictionary<ColorSO, int> comptador = new Dictionary<ColorSO, int>();
        foreach (var color in colorsAssignats)
        {
            if (!comptador.ContainsKey(color)) comptador[color] = 0;
            comptador[color]++;
        }
        foreach (var kvp in comptador)
            Debug.Log($"Color {kvp.Key.name}: {kvp.Value} grups assignats");
    }

    public void PaintGroup(WhatColorAmI triggeredPiece, ColorSO color)
    {
        if (!pieceToGroup.TryGetValue(triggeredPiece, out int groupId)) return;
        if (!groupMap.TryGetValue(groupId, out var pieces)) return;

        foreach (var piece in pieces)
            piece.PaintDirect(color);

        // Comprova si totes les peces estan pintades
        if (whatColorAmI.All(p => p.IsPainted))
            RoundManager.Instance?.OnFiguraCompletada();
    }

    // Crida això des de qualsevol lloc quan vols avançar de ronda
    public void IniciarRonda(int roundIndex)
    {
        currentRound = roundIndex;

        foreach (var piece in whatColorAmI)
            piece.ResetPece();

        BuildGroupMap();
        GenerarPaleta();
    }

    public void SeguentRonda()
    {
        IniciarRonda(currentRound + 1);
    }

    public bool HiHaMesRondes()
    {
        return currentRound + 1 < colorGroupsPerRound.Count;
    }

    public void NotificarColorActiu(ColorSO color)
    {
        OnColorActiuCanviat?.Invoke(color);
    }
    public void SetPaletes(List<PaletteSO> novesPaletes)
    {
        palettes = novesPaletes;
    }
}