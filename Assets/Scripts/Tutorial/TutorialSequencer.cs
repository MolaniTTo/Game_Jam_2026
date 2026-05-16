using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TutorialSequencer : MonoBehaviour
{
    public static TutorialSequencer Instance { get; private set; }

    [Header("Tutorial")]
    [SerializeField] private TutorialEntry tutorialEntry;

    [Header("Sargantana tutorial")]
    [SerializeField] private GameObject sargantanaTutorial; // la sargantana que corre hacia el player
    [SerializeField] private float arriveDistance = 1.5f;   // distancia a la que se para y mira

    [Header("Referencias")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform sargantanaTalkPoint;
    [SerializeField] private DracSpawner spawner;
    [SerializeField] private Image canvasHUD;

    [Header("Puntos de cámara (en orden)")]
    [SerializeField] private CinemachineCameraPoint[] cameraPoints;

    private bool triggered = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        canvasHUD.enabled = false; // Asegura que el HUD esté oculto al inicio del tutorial
    }

    // Llamado por TutorialZone cuando el player entra
    public void StartTutorial(Transform playerTransform)
    {
        if (triggered) return;
        triggered = true;
        StartCoroutine(TutorialSequence(playerTransform));
    }

    private IEnumerator TutorialSequence(Transform playerTransform)
    {
        // 1. Congela al player
        playerController.SetFrozen(true);

        // 2. Manda la sargantana tutorial correr hacia el player
        NavMeshAgent agent = sargantanaTutorial.GetComponent<NavMeshAgent>();
        Wonder wonder = sargantanaTutorial.GetComponent<Wonder>();

        if (wonder != null) wonder.enabled = false; // para el wandering
        if (agent != null) agent.SetDestination(sargantanaTalkPoint.position);

        // Espera a que llegue cerca del player
        yield return new WaitUntil(() =>
            agent == null ||
            (!agent.pathPending && agent.remainingDistance <= arriveDistance));

        // 3. La sargantana mira al player
        if (sargantanaTutorial != null)
        {
            Vector3 dir = (playerTransform.position - sargantanaTutorial.transform.position);
            dir.y = 0;
            if (dir != Vector3.zero)
                sargantanaTutorial.transform.rotation = Quaternion.LookRotation(dir);

            if (agent != null) agent.isStopped = true;
        }

        // Pequeña pausa dramática
        yield return new WaitForSeconds(0.5f);

        // 4. Arranca el diálogo — cuando acabe, onDone lanza el juego
        DracSpeaker.Instance.Speak(tutorialEntry, onDone: () =>
        {
            StartCoroutine(FinishTutorial());
        });
    }

    private IEnumerator FinishTutorial()
    {
        // La sargantana tutorial se va / desaparece
        if (sargantanaTutorial != null)
        {
            NavMeshAgent agent = sargantanaTutorial.GetComponent<NavMeshAgent>();
            if (agent != null) agent.isStopped = false;
            // Opcional: animación de salida, destroy tras un tiempo, etc.
            Destroy(sargantanaTutorial, 2f);
        }

        yield return new WaitForSeconds(0.3f);

        // Descongela al player
        playerController.SetFrozen(false);

        // Activa el spawner de sargantanas → empieza el juego
        if (spawner != null)
            spawner.StartSpawning();

        playerController.tutorialDone = true;
        canvasHUD.enabled = true; // Muestra el HUD al finalizar el tutorial
    }

    public CinemachineCameraPoint GetCameraPoint(int index)
    {
        if (index < 0 || index >= cameraPoints.Length) return null;
        return cameraPoints[index];
    }
}