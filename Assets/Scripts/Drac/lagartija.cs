using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Wonder : MonoBehaviour
{
    [Header("Opcional: puntos manuales (si no usa WaypointZone)")]
    public List<DracPoint> puntosPatrulla;

    private NavMeshAgent agent;
    private DracPoint puntoActual = null;
    public float distanciaMinima = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (puntosPatrulla == null || puntosPatrulla.Count == 0)
        {
            if (WaypointZone.Instance != null)
                puntosPatrulla = WaypointZone.Instance.GetPoints();
        }

        if (puntosPatrulla != null && puntosPatrulla.Count > 0)
            GoToWayPoint();
        else
            Debug.LogWarning("Wonder: no hay puntos para " + gameObject.name);
    }

    void Update()
    {
        if (!agent.enabled) return;
        if (!agent.pathPending && agent.remainingDistance < distanciaMinima)
            GoToWayPoint();
    }

    void GoToWayPoint()
    {
        if (puntoActual != null)
        {
            puntoActual.Release();
            puntoActual = null;
        }

        List<DracPoint> shuffled = new List<DracPoint>(puntosPatrulla);
        Shuffle(shuffled);

        foreach (DracPoint punto in shuffled)
        {
            if (punto.TryOccupy())
            {
                puntoActual = punto;
                agent.SetDestination(punto.transform.position);
                return;
            }
        }

        Invoke(nameof(GoToWayPoint), 1f);
    }

    void OnDestroy()
    {
        if (puntoActual != null)
            puntoActual.Release();
    }

    private void Shuffle(List<DracPoint> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}