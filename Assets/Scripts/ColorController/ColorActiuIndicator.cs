using UnityEngine;
using UnityEngine.UI;

public class ColorActiuIndicator : MonoBehaviour
{
    [SerializeField] private Image imagen;

    private void Awake()
    {
        if (imagen == null) imagen = GetComponent<Image>();
        SetAlpha(0f);
    }

    private void Update()
    {
        if (VisualDracColor.Instance == null) return;

        ColorSO colorActiu = VisualDracColor.Instance.ColorActiu;
        if (colorActiu != null)
        {
            Color c = colorActiu.color;
            c.a = 1f;
            imagen.color = c;
            SetAlpha(1f);
        }
        else
        {
            SetAlpha(0f);
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = imagen.color;
        c.a = alpha;
        imagen.color = c;
    }
}