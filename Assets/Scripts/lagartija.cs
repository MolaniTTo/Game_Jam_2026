using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic; // <--- ESTA ES LA QUE TE FALTA

public class Wonder : MonoBehaviour
{

    [Header("Configuración de Puntos")]
    public List<Transform> puntosPatrulla;
    private NavMeshAgent agent;

    [Header("Ajustes")]
    public float distanciaMinima = 0.5f;
    private int ultimoIndice = -1;
    private int penultimoIndice = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (puntosPatrulla.Count > 0)
        {
            GoToWayPoint();
        }
        else
        {
            Debug.LogError("¡No hay puntos asignados en la lista!");
        }
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < distanciaMinima)
        {
            GoToWayPoint();
        }
    }

    void GoToWayPoint()
    {
        if (puntosPatrulla.Count == 0) return;

        if (puntosPatrulla.Count <= 2)
        {
            int indice;
            do {
                indice = Random.Range(0, puntosPatrulla.Count);
            } while (indice == ultimoIndice && puntosPatrulla.Count > 1);
            
            ultimoIndice = indice;
            agent.SetDestination(puntosPatrulla[ultimoIndice].position);
            return;
        }

        int indiceAleatorio;

        do
        {
            indiceAleatorio = Random.Range(0, puntosPatrulla.Count);
        } while (indiceAleatorio == ultimoIndice || indiceAleatorio == penultimoIndice);

        penultimoIndice = ultimoIndice;
        ultimoIndice = indiceAleatorio;

        agent.SetDestination(puntosPatrulla[ultimoIndice].position);
    }
}
