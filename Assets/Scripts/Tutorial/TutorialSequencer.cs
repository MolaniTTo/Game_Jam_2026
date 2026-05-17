using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TutorialSequencer : MonoBehaviour
{
    public static TutorialSequencer Instance { get; private set; }

    [Header("Sargantana tutorial")]
    [SerializeField] private GameObject sargantanaTutorial;
    [SerializeField] private float arriveDistance = 1.5f;

    [Header("Referències")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform sargantanaTalkPoint;
    [SerializeField] private Image canvasHUD;

    [Header("Punts de càmera")]
    [SerializeField] private CinemachineCameraPoint[] cameraPoints;

    private Vector3 sargantanaStartPos;
    private Quaternion sargantanaStartRot;

    private Action onRondaAcabadaCallback;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        if(sargantanaTutorial != null)
        {
            sargantanaStartPos = sargantanaTutorial.transform.position;
            sargantanaStartRot = sargantanaTutorial.transform.rotation;
        }
    }

    private void Start()
    {
        canvasHUD.enabled = false;
    }

    // Ara rep el TutorialEntry i el callback des de RoundManager
    public void StartRonda(TutorialEntry entry, Action onAcabat)
    {
        onRondaAcabadaCallback = onAcabat;
        StartCoroutine(TutorialSequence(entry, playerController.transform));
    }

    private IEnumerator TutorialSequence(TutorialEntry entry, Transform playerTransform)
    {
        AudioManager.Instance.PlayMusic("TutorialDialogue", 1f);
        if (sargantanaTutorial != null)
        {
            sargantanaTutorial.transform.position = sargantanaStartPos;
            sargantanaTutorial.transform.rotation = sargantanaStartRot;
            sargantanaTutorial.SetActive(true);
        }

        playerController.SetFrozen(true);

        NavMeshAgent agent = sargantanaTutorial != null
            ? sargantanaTutorial.GetComponent<NavMeshAgent>()
            : null;
        Wonder wonder = sargantanaTutorial != null
            ? sargantanaTutorial.GetComponent<Wonder>()
            : null;

        if (wonder != null) wonder.Stop();
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        if (agent != null)
        {
            yield return new WaitUntil(() => agent.isOnNavMesh);
            agent.SetDestination(sargantanaTalkPoint.position);
            yield return null;
            yield return null;
            yield return new WaitUntil(() =>
                !agent.pathPending && agent.remainingDistance <= arriveDistance);

            agent.isStopped = true;
            agent.updateRotation = false;
            agent.velocity = Vector3.zero;

            Vector3 dir = (playerTransform.position - sargantanaTutorial.transform.position);
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                while (Quaternion.Angle(sargantanaTutorial.transform.rotation, targetRot) > 1f)
                {
                    sargantanaTutorial.transform.rotation = Quaternion.RotateTowards(
                        sargantanaTutorial.transform.rotation, targetRot, 200f * Time.deltaTime);
                    yield return null;
                }
                sargantanaTutorial.transform.rotation = targetRot;
                if (wonder != null) wonder.SetIdleAnimation();
                yield return new WaitForSeconds(0.5f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        DracSpeaker.Instance.Speak(entry, onDone: () =>
        {
            StartCoroutine(FinishTutorial());
        });
    }

    private IEnumerator FinishTutorial()
    {
        if (sargantanaTutorial != null)
        {
            NavMeshAgent agent = sargantanaTutorial.GetComponent<NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.updateRotation = true;
                agent.isStopped = false;
            }
            sargantanaTutorial.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);

        playerController.SetFrozen(false);
        playerController.tutorialDone = true;
        canvasHUD.enabled = true;

        // Notifica al RoundManager que el tutorial ha acabat
        onRondaAcabadaCallback?.Invoke();
    }

    public CinemachineCameraPoint GetCameraPoint(int index)
    {
        if (index < 0 || index >= cameraPoints.Length) return null;
        return cameraPoints[index];
    }
}