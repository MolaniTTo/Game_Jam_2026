using UnityEngine;

public class DialogueMusicController : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private string dialogueMusicKey = "Dialogue";
    [SerializeField] private string returnMusicKey = "Base"; //musica a la que ha de tornar un cop el boss es derrotat
    [SerializeField] private float fadeTime = 2f;
    [SerializeField] private bool startOnAwake = false; //comencar musica al iniciar
    [SerializeField] private bool useTransitionMusic = false; //utilitzar musica de transicio/victoria
    [SerializeField] private string transitionMusicKey = "Base"; //musica de transicio/victoria

    private RoundManager Instance; //referencia al RoundManager per saber quan entrem en dialeg

    private bool musicStarted = false;

    void Start()
    {
        if (startOnAwake)
        {
            StartDialogueMusic();
        }
    }

    public void StartDialogueMusic() //ho cridarem desde el gorila quan acaba WakeUp i desde el monje quan li tira el primer raig
    {
        if (musicStarted) return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(dialogueMusicKey, fadeTime);
            musicStarted = true;
        }
    }

    private void OnDialogueEnd()
    {

        if (AudioManager.Instance != null)
        {
            if (useTransitionMusic)
            {
                //reproducir musica de transicio
                AudioManager.Instance.PlayMusic(transitionMusicKey, fadeTime);

                //torna a la musica normal despres de 5 segons
                Invoke(nameof(ReturnToNormalMusic), 5f);
            }
            else
            {
                //torna directament a la musica normal
                ReturnToNormalMusic();
            }
        }

    }

    public void ReturnToNormalMusic() 
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(returnMusicKey, fadeTime);
        }
    }

}