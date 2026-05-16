using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Botones del Menú")]
    [SerializeField] private Button botonJugar;
    [SerializeField] private Button botonControles;
    [SerializeField] private Button botonSortir;
    [SerializeField] private Button botonCerrarControls;

    [Header("Paneles (Opcional)")]
    [SerializeField] private GameObject panelControles;
    [SerializeField] private GameObject panelPrincipal;

    private void Start()
    {
        // Asignar las funciones a los botones
        if (botonJugar != null)
            botonJugar.onClick.AddListener(Jugar);
        
        if (botonControles != null)
            botonControles.onClick.AddListener(MostrarControles);
        
        if (botonSortir != null)
            botonSortir.onClick.AddListener(Sortir);

        if (botonCerrarControls != null)
        {
            botonCerrarControls.onClick.AddListener(CerrarControles);
        }

        // Asegurar que solo el panel principal esté visible al inicio
        if (panelPrincipal != null)
            panelPrincipal.SetActive(true);
        
        if (panelControles != null)
            panelControles.SetActive(false);
    }

    // Método para el botón Jugar
    public void Jugar()
    {
        Debug.Log("Cargando juego...");
        // Reemplaza "NombreDeTuEscena" con el nombre real de tu escena de juego
        SceneManager.LoadScene("MainWorld");
    }

    public void CerrarControles()
    {
        panelControles.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    // Método para el botón Controles
    public void MostrarControles()
    {
        Debug.Log("Mostrando controles...");
        
        // Opción 1: Ocultar menú principal y mostrar panel de controles
        if (panelPrincipal != null && panelControles != null)
        {
            panelPrincipal.SetActive(false);
            panelControles.SetActive(true);
        }
    }

    // Método para ocultar los controles y volver al menú principal
    public void OcultarControles()
    {
        if (panelPrincipal != null && panelControles != null)
        {
            panelPrincipal.SetActive(true);
            panelControles.SetActive(false);
        }
    }

    // Método para el botón Sortir (Salir)
    public void Sortir()
    {
        Debug.Log("Saliendo del juego...");
        
        #if UNITY_EDITOR
            // Si estamos en el editor de Unity
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Si es una build del juego
            Application.Quit();
        #endif
    }

    // Opcional: Método para limpiar los listeners cuando el objeto se destruye
    private void OnDestroy()
    {
        if (botonJugar != null)
            botonJugar.onClick.RemoveListener(Jugar);
        
        if (botonControles != null)
            botonControles.onClick.RemoveListener(MostrarControles);
        
        if (botonSortir != null)
            botonSortir.onClick.RemoveListener(Sortir);
    }
}