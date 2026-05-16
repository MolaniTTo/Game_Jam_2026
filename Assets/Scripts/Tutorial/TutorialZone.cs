using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialSequencer.Instance?.StartTutorial(other.transform);
            // Desactiva el trigger para que no se repita
            GetComponent<Collider>().enabled = false;
        }
    }
}