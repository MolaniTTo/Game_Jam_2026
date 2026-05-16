using UnityEngine;

public class Manotazo : MonoBehaviour
{
    private bool girando = false;
    private float velocidad = 450f;

    private Quaternion rotacionInicial;
    private float rotacionActual = 0f;

    private MeshRenderer meshRenderer;

    private int sentido = -1;
    private ParticleSystem particleSystem;

    void Start()
    {
        rotacionInicial = transform.localRotation;
        meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

        meshRenderer.enabled = false;

        particleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (!girando) return;

        float rotacionFrame = velocidad * Time.deltaTime;

        rotacionActual += rotacionFrame;

        transform.localRotation =
            rotacionInicial * Quaternion.Euler(0, sentido * rotacionActual, 0);

        if (rotacionActual >= 180f)
        {
            girando = false;
            rotacionActual = 0f;
            transform.localRotation = rotacionInicial;
            meshRenderer.enabled = false;
        }
    }

    public void Ejecutar()
    {
        if (girando) return;

        meshRenderer.enabled = true;

        girando = true;
        rotacionActual = 0f;

        particleSystem.Play();
    }
}