using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Material scultureMat;
    public Color otherColor;
    public string otherColorName;

    private Material instanceMat; //instancia propia de lagartija o escultura
    private Renderer lizardRenderer;


    void Start()
    {
        otherColor = Color.green;
        lizardRenderer = GetComponent<Renderer>();

        if (lizardRenderer != null)
        {
            instanceMat = new Material(lizardRenderer.material); //crea una nueva instancia del material para evitar modificar el material original
            lizardRenderer.material = instanceMat; //asigna la nueva instancia al renderer del lagarto
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ColorTerra>() != null && gameObject.CompareTag("sargantana"))
        {
            ColorTerra terra = other.gameObject.GetComponent<ColorTerra>();
            otherColor = terra.colorSO.color;
            otherColorName = terra.colorSO.colorName;
            instanceMat.color = otherColor; //cambia el color del material del lagarto al color del ScriptableObject
        }
    }

    public Color GetCurrentColor()
    {
        return otherColor; //retorna el nombre del color actual del lagarto
    }

    public void ChangeColorSculture(Color newColor)
    {
        if(instanceMat != null)
        {
            instanceMat.color = newColor; //cambia el color del material de la escultura al nuevo color recibido
        }
    }
}
