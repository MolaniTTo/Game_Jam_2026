using UnityEngine;

public class ColorPedra : MonoBehaviour
{
    [SerializeField] private Renderer piedraRenderer;
    private Material instanceMat;
    private ColorTerra colorTerra;

    void Awake()
    {
        if (piedraRenderer == null)
            piedraRenderer = GetComponent<Renderer>();

        if (piedraRenderer != null)
        {
            instanceMat = new Material(piedraRenderer.material);
            piedraRenderer.material = instanceMat;
        }
        else
        {
            Debug.LogError("ColorPiedra: no s'ha trobat Renderer a " + gameObject.name);
        }
    }

    private void Start()
    {
        colorTerra = gameObject.GetComponent<ColorTerra>();
        if (colorTerra == null)
        {
            Debug.LogError("ColorPedra: no s'ha trobat ColorTerra a " + gameObject.name);
        }
        if (colorTerra != null)
        {
            SetColor(colorTerra.colorSO.color);
        }
    }

    public void SetColor(Color color)
    {
        if (instanceMat != null)
            instanceMat.color = color;
    }
}