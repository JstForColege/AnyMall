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
    [SerializeField] private int maxCustomers = 8;

    private List<CustomerAI> activeCustomers = new List<CustomerAI>();

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
        if (activeCustomers.Count >= maxCustomers)
        {
            Debug.Log($"CustomerSpawner: Max customers ({maxCustomers}) reached");
            return;
        }

        if (customerPrefab == null || spawnPoint == null) return;
        if (waypoints == null || waypoints.Count == 0) return;

        GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        CustomerAI customerAI = newCustomer.GetComponent<CustomerAI>();
        if (customerAI != null)
        {
            int count = Random.Range(2, Mathf.Min(waypoints.Count + 1, 4));

            List<Transform> selectedPoints = new List<Transform>();
            List<ItemType> shoppingList = new List<ItemType>();
            List<Transform> available = new List<Transform>(waypoints);

            for (int i = 0; i < count; i++)
            {
                if (available.Count == 0) break;
                int idx = Random.Range(0, available.Count);
                Transform point = available[idx];
                selectedPoints.Add(point);
                available.RemoveAt(idx);

                Storage shelf = point.GetComponent<Storage>();
                if (shelf != null)
                {

                    shoppingList.Add(shelf.GetItemType());
                }
            }

            if (selectedPoints.Count > 0 && Vector3.Distance(spawnPoint.position, selectedPoints[0].position) < 0.5f)
            {
                Transform first = selectedPoints[0];
                selectedPoints.RemoveAt(0);
                if (available.Count > 0)
                {
                    selectedPoints.Insert(0, available[Random.Range(0, available.Count)]);
                }
                else
                {
                    selectedPoints.Add(first);
                }
            }

            customerAI.SetWaypoints(selectedPoints);
            customerAI.SetShoppingList(shoppingList);
            customerAI.SetCashPoint(cashWaypoint);

            activeCustomers.Add(customerAI);
            customerAI.SetSpawner(this);
        }
    }

    public void OnCustomerLeft(CustomerAI customer)
    {
        if (activeCustomers.Contains(customer))
        {
            activeCustomers.Remove(customer);
        }
    }
}