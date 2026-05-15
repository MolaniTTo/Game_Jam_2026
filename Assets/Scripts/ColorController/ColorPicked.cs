using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicked : MonoBehaviour
{
    public Action onDragonPicked; 
    public string currentColor; //public 
    public Drac drac;
    public ColorSO colorSO; //el ScriptableObject on guardarem el color del llangardaix


    void Start()
    {
        onDragonPicked += DragonPicked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DragonPicked()
    {
        currentColor = drac.changeColor.GetCurrentColor();
        GetColorInRGB(currentColor);

    }

    public void GetColorInRGB(string colorName)
    {
        Color colorRGB = Color.white; // Default color

        switch (colorName)
        {
            case "Vermell":
                colorRGB = Color.red;
                break;
            case "Verd":
                colorRGB = Color.green;
                break;
            case "blue":
                colorRGB = Color.blue;
                break;
            case "yellow":
                colorRGB = Color.yellow;
                break;
            case "cyan":
                colorRGB = Color.cyan;
                break;
            case "magenta":
                colorRGB = Color.magenta;
                break;
            case "white":
                colorRGB = Color.white;
                break;
            case "black":
                colorRGB = Color.black;
                break;
            default:
                Debug.LogWarning("Color not recognized: " + colorName);
                break;
        }
    }
}
