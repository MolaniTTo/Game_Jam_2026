using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Palette", menuName = "ScriptableObjects/Palette")]
public class PaletteSO : ScriptableObject
{
    public string paletteName;
    public List<ColorSO> colors;
}