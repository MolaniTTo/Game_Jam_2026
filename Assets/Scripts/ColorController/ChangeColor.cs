using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [SerializeField] private Renderer lizardRenderer;
    public Color otherColor;
    public string otherColorName;
    private Material instanceMat; //instancia propia de lagartija o escultura


    void Awake()
    {
        otherColor = Color.green;

        if (lizardRenderer != null)
        {
            instanceMat = new Material(lizardRenderer.material);
            lizardRenderer.material = instanceMat;
            Debug.Log("ChangeColor OK: " + gameObject.name);
        }
        else
        {
            Debug.LogError("ChangeColor: lizardRenderer es NULL en " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ColorTerra>() != null && gameObject.CompareTag("sargantana"))
        {
            if (instanceMat == null)
            {
                Debug.LogError("instanceMat es NULL en " + gameObject.name + " — ¿está asignado el Renderer en el Inspector?");
                return;
            }

            ColorTerra terra = other.gameObject.GetComponent<ColorTerra>();
            otherColor = terra.colorSO.color;
            otherColorName = terra.colorSO.colorName;
            instanceMat.color = otherColor;
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

    public void ChangeColorSargantana(Color newColor)
    {
        if (instanceMat != null)
        {
            instanceMat.color = newColor; //cambia el color del material de la escultura al nuevo color recibido
        }
    }
}
