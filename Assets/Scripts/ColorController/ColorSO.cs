using UnityEngine;

[CreateAssetMenu(fileName = "ColorTerra", menuName = "ScriptableObjects/ColorTerra")]
public class ColorSO : ScriptableObject
{
    public Color color;
    public string colorName;
    public Color GetColor()
    {
        return color;
    }
}
