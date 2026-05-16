using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [System.Serializable]
    public class RondaConfig
    {
        public TutorialEntry tutorialEntry;        // diàleg primera vegada
        public TutorialEntry tutorialEntryRetry;   // diàleg si repeteixes la ronda
        public List<PaletteSO> paletesPossibles;
        public int maxSargantanas;
        public float tempsRonda;
        public Transform playerSpawnPoint;
    }

    [Header("Configuració de rondes")]
    [SerializeField] private List<RondaConfig> rondes;

    [Header("Referències")]
    [SerializeField] private TutorialSequencer tutorialSequencer;
    [SerializeField] private DracSpawner dracSpawner;
    [SerializeField] private RoundController roundController;
    [SerializeField] private TempsRestant tempsRestant;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private VisualDracColor visualDracColor;

    private int currentRoundIndex = 0;
    private bool rondaActiva = false;
    private bool transitant = false;
    private bool primerCopRonda = true; // true = primera vegada, false = reintent

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        IniciarRonda(0, primerCop: true);
    }

    public void IniciarRonda(int index, bool primerCop)
    {
        if (index >= rondes.Count)
        {
            Debug.Log("[RoundManager] Totes les rondes completades!");
            return;
        }

        currentRoundIndex = index;
        primerCopRonda = primerCop;
        rondaActiva = false;
        transitant = false;

        RondaConfig config = rondes[currentRoundIndex];

        // Teleporta el jugador
        if (config.playerSpawnPoint != null)
        {
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            playerController.transform.position = config.playerSpawnPoint.position;
            playerController.transform.rotation = config.playerSpawnPoint.rotation;
        }

        roundController.SetPaletes(config.paletesPossibles);
        roundController.IniciarRonda(currentRoundIndex);
        dracSpawner.ConfigurarRonda(config.maxSargantanas);
        tempsRestant.ConfigurarRonda(config.tempsRonda);

        // Escull el diàleg segons si és reintent o no
        TutorialEntry entryAMostrar = (primerCop || config.tutorialEntryRetry == null)
            ? config.tutorialEntry
            : config.tutorialEntryRetry;

        tutorialSequencer.StartRonda(entryAMostrar, OnTutorialAcabat);
    }

    private void OnTutorialAcabat()
    {
        rondaActiva = true;
        dracSpawner.StartSpawning();
        tempsRestant.StartTimer();
        visualDracColor.IniciarCicle(); // arranca el visual DESPRÉS del tutorial
    }

    public void OnTempsAcabat()
    {
        if (!rondaActiva || transitant) return;
        transitant = true;
        Debug.Log("[RoundManager] Temps acabat — repetint ronda");
        StartCoroutine(TransicioRonda(completada: false));
    }

    public void OnFiguraCompletada()
    {
        if (!rondaActiva || transitant) return;
        transitant = true;
        Debug.Log("[RoundManager] Figura completada!");
        StartCoroutine(TransicioRonda(completada: true));
    }

    private IEnumerator TransicioRonda(bool completada)
    {
        rondaActiva = false;
        tempsRestant.StopTimer();
        dracSpawner.StopSpawning();
        visualDracColor.AturaciCicle();

        foreach (var s in GameObject.FindGameObjectsWithTag("sargantana"))
            Destroy(s);

        yield return new WaitForSeconds(1.5f);

        if (completada)
            IniciarRonda(currentRoundIndex + 1, primerCop: true);
        else
            IniciarRonda(currentRoundIndex, primerCop: false); // mateixa ronda, reintent
    }

    public int GetCurrentRoundIndex() => currentRoundIndex;
}