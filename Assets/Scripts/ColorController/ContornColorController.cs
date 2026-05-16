using UnityEngine;
using UnityEngine.UI;

public class ContornColorController : MonoBehaviour
{
    private Image imagenUI;

    private void Start() {
        imagenUI = gameObject.GetComponent<Image>();

        Color defaultColor = new Color(255f, 0f, 0f, 100f);
        CanviarColor(defaultColor);
    }

    public void CanviarColor(Color nouColor)
    {
        imagenUI.color = nouColor;
    }
}
