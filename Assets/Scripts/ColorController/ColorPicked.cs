using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicked : MonoBehaviour
{
    public Color currentColor; //public 
    public ColorSO colorSO; //el ScriptableObject on guardarem el color del llangardaix


    void Start()
    {

    }

    public void DragonPicked(Drac drac)
    {
        currentColor = drac.changeColor.GetCurrentColor();
        currentColor = colorSO.color;
    }

}
