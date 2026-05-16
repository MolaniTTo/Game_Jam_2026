// TutorialEntry.cs
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialEntry", menuName = "Tutorial/TutorialEntry")]
public class TutorialEntry : ScriptableObject
{
    public TutorialLine[] lines;
}

[System.Serializable]
public class TutorialLine
{
    [TextArea(2, 5)]
    public string text;

    // -1 = sin blend de cámara, 0,1,2... = índice en la lista de puntos del TutorialSequencer
    public int cameraPointIndex = -1;
    public float cameraHoldSeconds = 2f;
}