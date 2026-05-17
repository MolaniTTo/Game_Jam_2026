using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [System.Serializable]
    public class RondaConfig
    {
        public TutorialEntry tutorialEntry;
        public TutorialEntry tutorialEntryRetry;
        public List<PaletteSO> paletesPossibles;
        public int maxSargantanas;
        public float tempsRonda;
        public float speed;
        public Transform playerSpawnPoint;
        public ProgressColorHUD hudPrefabRonda; // prefab HUD específic per aquesta ronda
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
    [SerializeField] private ScreenFade screenFade;
    [SerializeField] private GameObject timerObj;

    [Header("Final del joc")]
    [SerializeField] private string nomEscenaFinal = "Credits";
    [SerializeField] private float tempsEntrePeces = 0.1f; // temps entre cada peça que es pinta
    [SerializeField] private float tempsAbansCanviEscena = 2f;

    private int currentRoundIndex = 0;
    private bool rondaActiva = false;
    private bool transitant = false;
    private bool primerCopRonda = true;

    private ProgressColorHUD hudActiu = null; // HUD actiu ara mateix
    public TextMeshProUGUI rondaText; // Text que mostra el número de ronda actual
    private bool EsUltimaRonda => currentRoundIndex >= rondes.Count - 1; 

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        screenFade.FadeIn();
        IniciarRonda(0, primerCop: true);
    }

    public void IniciarRonda(int index, bool primerCop)
    {
        if (index >= rondes.Count)
        {
            Debug.Log("[RoundManager] Totes les rondes completades!");
            return;
        }

        // Desactiva HUD anterior
        if (hudActiu != null)
        {
            hudActiu.Reset();
            hudActiu.gameObject.SetActive(false);
            hudActiu = null;
        }
        if (timerObj != null) timerObj.SetActive(false);

        currentRoundIndex = index;
        ActualitzarTextRonda();
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
        dracSpawner.ConfigurarRonda(config.maxSargantanas, config.speed);

        // Si és l'última ronda, pinta el drac i arranca el diàleg final
        if (EsUltimaRonda)
        {
            StartCoroutine(SequenciaFinal(config));
            return;
        }

        // Comportament normal
        TutorialEntry entryAMostrar = (primerCop || config.tutorialEntryRetry == null)
            ? config.tutorialEntry
            : config.tutorialEntryRetry;
        string musicaTutorial = (primerCop || config.tutorialEntryRetry == null)
            ? "TutorialDialogue"
            : "TutorialFailed";
        AudioManager.Instance.PlayMusic(musicaTutorial, 1f);

        tutorialSequencer.StartRonda(entryAMostrar, OnTutorialAcabat);

        tutorialSequencer.StartRonda(entryAMostrar, OnTutorialAcabat);
    }

    private void ActivarHUD(RondaConfig config)
    {
        if (timerObj != null)
        {
            timerObj.SetActive(true);
        }
        // Desactiva el HUD anterior
        if (hudActiu != null)
        {
            hudActiu.Reset();
            hudActiu.gameObject.SetActive(false);
        }

        // Activa el nou
        if (config.hudPrefabRonda != null)
        {
            hudActiu = config.hudPrefabRonda;
            hudActiu.gameObject.SetActive(true);
        }
    }

    private void OnTutorialAcabat()
    {
        RondaConfig config = rondes[currentRoundIndex];
        ActivarHUD(config);
        // Comportament normal
        tempsRestant.ConfigurarRonda(config.tempsRonda);
        if (hudActiu != null)
            hudActiu.Inicialitzar(roundController.colorsResultat, roundController.whatColorAmI);

        rondaActiva = true;
        dracSpawner.StartSpawning();
        tempsRestant.StartTimer();
        visualDracColor.IniciarCicle();
        AudioManager.Instance.PlayMusic("Base" , 2f);
    }

    // Cridat per WhatColorAmI quan una peça es pinta correctament
    public void NotificarPecaPintada(ColorSO color)
    {
        hudActiu?.NotificarPecaPintada(color);
    }

    public void OnTempsAcabat()
    {
        if (!rondaActiva || transitant) return;
        transitant = true;
        AudioManager.Instance.StopMusic(1f);
        StartCoroutine(TransicioRonda(completada: false));
    }

    public void OnFiguraCompletada()
    {
        if (!rondaActiva || transitant) return;
        transitant = true;
        AudioManager.Instance.StopMusic(1f);
        StartCoroutine(TransicioRonda(completada: true));
    }

    private IEnumerator TransicioRonda(bool completada)
    {
        rondaActiva = false;
        tempsRestant.StopTimer();
        dracSpawner.StopSpawning();
        visualDracColor.AturaciCicle();

        screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeDuration);

        foreach (var s in GameObject.FindGameObjectsWithTag("sargantana"))
            Destroy(s);

        if (completada)
            IniciarRonda(currentRoundIndex + 1, primerCop: true);
        else
            IniciarRonda(currentRoundIndex, primerCop: false);

        yield return new WaitForSeconds(0.2f);
        screenFade.FadeIn();
    }

    private IEnumerator SequenciaFinal(RondaConfig config)
    {
        // Pinta totes les peces una per una
        foreach (WhatColorAmI peca in roundController.whatColorAmI)
        {
            ColorSO color = peca.GetValidColor();
            if (color != null)
                peca.PaintDirect(color);
            yield return new WaitForSeconds(tempsEntrePeces);
        }

        yield return new WaitForSeconds(0.5f);

        AudioManager.Instance.PlayMusic("Win", 1f);
        // Ara arranca el diàleg final
        TutorialEntry entryFinal = config.tutorialEntry;
        tutorialSequencer.StartRonda(entryFinal, () =>
        {
            StartCoroutine(FinalAmbFade());
        });
    }

    private IEnumerator FinalAmbFade()
    {
        yield return new WaitForSeconds(tempsAbansCanviEscena);
        screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeDuration);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nomEscenaFinal);
    }

    public int GetCurrentRoundIndex() => currentRoundIndex;

    private void ActualitzarTextRonda()
    {
        if (rondaText != null)
            rondaText.text = "Ronda: " + (currentRoundIndex + 1);
    }
}