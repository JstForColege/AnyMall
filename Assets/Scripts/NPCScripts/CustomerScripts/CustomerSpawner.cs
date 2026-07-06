using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private List<Transform> waypoints;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnCustomer();
        }
    }

    private void SpawnCustomer()
    {
        if (customerPrefab == null || spawnPoint == null) return;

        GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        CustomerAI customerAI = newCustomer.GetComponent<CustomerAI>();
        if (customerAI != null)
        {
            int count = Random.Range(2, Mathf.Min(5, waypoints.Count + 1));
            List<Transform> selected = new List<Transform>();
            for (int i = 0; i < count; i++)
            {
                Transform point = waypoints[Random.Range(0, waypoints.Count)];
                if (!selected.Contains(point))
                    selected.Add(point);
            }
            customerAI.SetWaypoints(selected);
        }
    }
}