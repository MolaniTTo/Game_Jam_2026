using System.Collections;
using UnityEngine;

public class DracSpawner : MonoBehaviour
{
    [SerializeField] private GameObject sargantanaPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int initialCount = 5;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private int maxSargantanas = 15;

    private bool spawning = false;

    public void StartSpawning()
    {
        if (spawning) return;
        spawning = true;

        // Spawn inicial
        for (int i = 0; i < initialCount; i++)
            SpawnOne();

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (GameObject.FindGameObjectsWithTag("sargantana").Length < maxSargantanas)
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