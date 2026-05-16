using System.Collections;
using UnityEngine;

public class DracSpawner : MonoBehaviour
{
    [SerializeField] private GameObject sargantanaPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int initialCount = 5;
    [SerializeField] private float spawnInterval = 10f;

    private int maxSargantanas = 15;
    private bool spawning = false;
    private Coroutine spawnCoroutine;

    public void ConfigurarRonda(int nouMax)
    {
        maxSargantanas = nouMax;
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
        Instantiate(sargantanaPrefab, point.position, point.rotation);
    }
}