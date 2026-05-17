using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DracSpawner : MonoBehaviour
{
    [SerializeField] private GameObject sargantanaPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int initialCount = 5;
    [SerializeField] private float spawnInterval = 10f;

    private int maxSargantanas = 15;
    private float speedActual = 3.5f; // valor per defecte
    private bool spawning = false;
    private Coroutine spawnCoroutine;

    public void ConfigurarRonda(int nouMax, float nouSpeed)
    {
        maxSargantanas = nouMax;
        speedActual = nouSpeed;
        spawning = false;
    }

    public void StartSpawning()
    {
        if (spawning) return;
        spawning = true;
        for (int i = 0; i < initialCount; i++)
            SpawnOne();
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        spawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (spawning)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (spawning && GameObject.FindGameObjectsWithTag("sargantana").Length < maxSargantanas)
                SpawnOne();
        }
    }

    private void SpawnOne()
    {
        if (spawnPoints.Length == 0) return;
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject nova = Instantiate(sargantanaPrefab, point.position, point.rotation);

        NavMeshAgent agent = nova.GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.speed = speedActual;
    }
}