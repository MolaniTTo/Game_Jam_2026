// TutorialEntry.cs
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorGroups", menuName = "Color/ColorGroups")]
public class ColorGroupsSO : ScriptableObject
{
    public ColorGroup[] groups; //Llista de grups de peces, cada grup conté varies llistes de peces que han de tenir el mateix color
}

[System.Serializable]
public class ColorGroup
{
    public List<string> pieceNames; //Llista de peces que han de tenir el mateix color
}
