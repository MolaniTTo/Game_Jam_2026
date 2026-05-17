using UnityEngine;
using TMPro;

public class TempsRestant : MonoBehaviour
{
    private TextMeshProUGUI textTemps;
    private float totalTime = 120f;
    private bool running = false;

    void Awake() // <-- canviat de Start a Awake
    {
        textTemps = GetComponent<TextMeshProUGUI>();
    }

    void FixedUpdate()
    {
        if (!running) return;
        if (totalTime > 0)
        {
            totalTime -= Time.deltaTime;
            if (totalTime <= 0)
            {
                totalTime = 0;
                running = false;
                ActualitzarText();
                RoundManager.Instance?.OnTempsAcabat();
                return;
            }
            ActualitzarText();
        }
    }

    private void ActualitzarText()
    {
        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);
        textTemps.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ConfigurarRonda(float temps)
    {
        totalTime = temps;
        running = false;
        ActualitzarText();
    }

    public void StartTimer() => running = true;
    public void StopTimer() => running = false;
}