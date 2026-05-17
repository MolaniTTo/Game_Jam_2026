using UnityEngine;
using UnityEngine.UI;

public class ColorActiuIndicator : MonoBehaviour
{
    [SerializeField] private Image imagen;
    [SerializeField] private float segonesAbansFinal = 3f;
    [SerializeField] private float velocitateParpadeo = 3f; // parpadeos per segon

    private void Awake()
    {
        if (imagen == null) imagen = GetComponent<Image>();
        SetAlpha(0f);
    }

    private void Update()
    {
        if (VisualDracColor.Instance == null) { SetAlpha(0f); return; }

        ColorSO colorActiu = VisualDracColor.Instance.ColorActiu;
        float tempsRestant = VisualDracColor.Instance.TempsRestantColor;

        if (colorActiu == null || tempsRestant > segonesAbansFinal)
        {
            SetAlpha(0f);
            return;
        }

        // Últims 3 segons — parpadeo suau amb sin
        Color c = colorActiu.color;
        c.a = 1f;
        imagen.color = c;

        float alpha = (Mathf.Sin(Time.time * velocitateParpadeo * Mathf.PI) + 1f) / 2f;
        SetAlpha(alpha);
    }

    private void SetAlpha(float alpha)
    {
        Color c = imagen.color;
        c.a = alpha;
        imagen.color = c;
    }
}