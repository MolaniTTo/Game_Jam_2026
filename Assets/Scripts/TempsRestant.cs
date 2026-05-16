using UnityEngine;
using TMPro;

public class TempsRestant : MonoBehaviour
{
    private TextMeshProUGUI textTemps;
    [SerializeField] float totalTime = 120f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textTemps = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (totalTime > 0)
        {
            totalTime -= Time.deltaTime;
            if (totalTime < 0) totalTime = 0;
            
            int minutes = Mathf.FloorToInt(totalTime / 60);
            int seconds = Mathf.FloorToInt(totalTime % 60);
            textTemps.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
