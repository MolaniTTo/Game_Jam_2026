using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuDePausaScript : MonoBehaviour
{
    [Header("Botones del Menú")]
    [SerializeField] private Button botonResumen;
    [SerializeField] private Button botonSortir;
    [SerializeField] private PlayerController player;

    [SerializeField] private GameObject panel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (botonResumen != null)
            botonResumen.onClick.AddListener(Resumen);
        
        if (botonSortir != null)
            botonSortir.onClick.AddListener(Sortir);

        panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {

        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Time.timeScale == 0f)
            {
                Resumen();
            }
            else
            {
                Pausa();
            }
        }
    }
    
    void Pausa()
    {
        panel.SetActive(true);
        player.isOnMenuMode = true;
        AudioManager.Instance.StopMusic(1f);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    void Resumen()
    {
        panel.SetActive(false);
        player.isOnMenuMode = false;
        AudioManager.Instance.PlayMusic("Base");
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Sortir()
    {
        Debug.Log("Sortir");
    }
}
