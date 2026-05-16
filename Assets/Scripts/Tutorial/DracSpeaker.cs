using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Cinemachine;

public class DracSpeaker : MonoBehaviour
{
    public static DracSpeaker Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private GameObject continuePrompt;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.03f;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineBrain cinemachineBrain;

    private TutorialLine[] currentLines;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool isSpeaking = false;
    private bool waitingForClick = false;
    private bool blockingForCamera = false; // bloquea el click durante el blend
    private string currentLineText = "";
    private System.Action onFinished;

    public bool IsSpeaking => isSpeaking;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        canvas.enabled = false;
        if (continuePrompt != null) continuePrompt.SetActive(false);
    }

    void Update()
    {
        if (!isSpeaking) return;
        if (blockingForCamera) return; 

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);

                //ensenya tot de cop
                textUI.ForceMeshUpdate();
                TMP_TextInfo textInfo = textUI.textInfo;
                for (int i = 0; i < textInfo.characterCount; i++)
                    SetCharAlpha(textInfo, i, 255);
                textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                isTyping = false;
                waitingForClick = true;
                if (continuePrompt != null) continuePrompt.SetActive(true);
            }
            else if (waitingForClick)
            {
                waitingForClick = false;
                currentIndex++;
                StartCoroutine(ShowLine(currentIndex));
            }
        }
    }

    public void Speak(TutorialEntry entry, System.Action onDone = null)
    {
        if (entry == null || entry.lines.Length == 0) return;
        currentLines = entry.lines;
        currentIndex = 0;
        onFinished = onDone;
        isSpeaking = true;
        canvas.enabled = true;
        StartCoroutine(ShowLine(0));
    }

    private IEnumerator ShowLine(int index)
    {
        if (index >= currentLines.Length) { Close(); yield break; }

        TutorialLine line = currentLines[index];

        if (line.cameraPointIndex >= 0)
        {
            var point = TutorialSequencer.Instance?.GetCameraPoint(line.cameraPointIndex);
            if (point != null)
            {
                blockingForCamera = true;
                canvas.enabled = false; //no es veu el text durant el blend
                yield return StartCoroutine(DoCameraBlend(point, line.cameraHoldSeconds));
                canvas.enabled = true;
                blockingForCamera = false;
            }
        }

        currentLineText = line.text;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }

    private IEnumerator DoCameraBlend(CinemachineCameraPoint point, float holdSeconds)
    {
        point.Activate();

        yield return null;
        yield return null;

        yield return new WaitUntil(() => !cinemachineBrain.IsBlending);

        yield return new WaitForSeconds(holdSeconds);

        point.Deactivate();

        yield return null;
        yield return null;

        yield return new WaitUntil(() => !cinemachineBrain.IsBlending);
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        if (continuePrompt != null) continuePrompt.SetActive(false);

        //escriu tot de cop pero no es veu
        textUI.text = line;
        textUI.ForceMeshUpdate(); 

        TMP_TextInfo textInfo = textUI.textInfo;
        int totalChars = textInfo.characterCount;

        //ho posa tot en alpha 0 (invisible)
        for (int i = 0; i < totalChars; i++)
            SetCharAlpha(textInfo, i, 0);
        textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        //va ensenyant un per un
        for (int i = 0; i < totalChars; i++)
        {
            SetCharAlpha(textInfo, i, 255);
            textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
        waitingForClick = true;
        if (continuePrompt != null) continuePrompt.SetActive(true);
    }

    private void SetCharAlpha(TMP_TextInfo textInfo, int charIndex, byte alpha)
    {
        //nomes caracters visibles, perque no quedi raro amb els espais
        if (!textInfo.characterInfo[charIndex].isVisible) return;

        int meshIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

        vertexColors[vertexIndex + 0].a = alpha;
        vertexColors[vertexIndex + 1].a = alpha;
        vertexColors[vertexIndex + 2].a = alpha;
        vertexColors[vertexIndex + 3].a = alpha;
    }

    private void Close()
    {
        isSpeaking = false;
        canvas.enabled = false;
        if (continuePrompt != null) continuePrompt.SetActive(false);
        textUI.text = "";
        onFinished?.Invoke();
        onFinished = null;
    }
}