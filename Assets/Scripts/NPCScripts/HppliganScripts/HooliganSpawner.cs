using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HooliganSpawner : MonoBehaviour
{
    [SerializeField] private GameObject hooliganPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minSpawnInterval = 15f;
    [SerializeField] private float maxSpawnInterval = 30f;
    [SerializeField] private List<Transform> shelves;

    private bool isSpawned = false;
    private HooliganAI currentHooligan;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(interval);
            SpawnHooligan();
        }
    }

    private void SpawnHooligan()
    {
        if (hooliganPrefab == null || spawnPoint == null) return;
        if (shelves == null || shelves.Count == 0) return;
        if (isSpawned) return;

        GameObject newHooligan = Instantiate(hooliganPrefab, spawnPoint.position, Quaternion.identity);
        HooliganAI hooliganAI = newHooligan.GetComponent<HooliganAI>();
        if (hooliganAI != null)
        {
            Transform target = shelves[Random.Range(0, shelves.Count)];
            hooliganAI.SetTargetShelf(target);
            hooliganAI.SetSpawner(this);
            isSpawned = true;
            currentHooligan = hooliganAI;
        }
    }

    public void OnHooliganLeft()
    {
        isSpawned = false;
        currentHooligan = null;
        Debug.Log("Hooligan: Spawner notified, can spawn new one");
    }
}