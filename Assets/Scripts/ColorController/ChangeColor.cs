using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Material lagartoMat;
    public Material scultureMat;
    public Color otherColor;
    public string otherColorName;


    void Start()
    {
        otherColor = Color.green;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ColorTerra>() != null && gameObject.CompareTag("sargantana"))
        {
            otherColor = other.gameObject.GetComponent<ColorTerra>().colorSO.color; //agafa el color del objeto que ha colisionado
            otherColorName = other.gameObject.GetComponent<ColorTerra>().colorSO.colorName; //agafa el nombre del color del objeto que ha colisionado
            lagartoMat.color = otherColor; //cambia el color del material del lagarto al color del objeto que ha colisionado
        }
    }

    public Color GetCurrentColor()
    {
        return otherColor; //retorna el nombre del color actual del lagarto
    }

    public void ChangeColorSculture(Color newColor)
    {
        scultureMat.color = newColor; 
    }
}
