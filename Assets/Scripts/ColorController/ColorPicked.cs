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
        colorSO = drac.changeColor.GetCurrentColorSO(); // guarda el ColorSO de la lagartija
        currentColor = colorSO != null ? colorSO.color : drac.changeColor.GetCurrentColor();
    }

}
