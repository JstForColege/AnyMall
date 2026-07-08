using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private Transform cashWaypoint;

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
        if (waypoints == null || waypoints.Count == 0) return;

        GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        CustomerAI customerAI = newCustomer.GetComponent<CustomerAI>();
        if (customerAI != null)
        {
            int count = Mathf.Min(Random.Range(2, 4), waypoints.Count);
            List<Transform> selected = new List<Transform>();
            List<Transform> available = new List<Transform>(waypoints);

            for (int i = 0; i < count; i++)
            {
                if (available.Count == 0) break;
                int idx = Random.Range(0, available.Count);
                selected.Add(available[idx]);
                available.RemoveAt(idx);
            }

            customerAI.SetWaypoints(selected);
            customerAI.SetCashPoint(cashWaypoint);
        }
    }
}