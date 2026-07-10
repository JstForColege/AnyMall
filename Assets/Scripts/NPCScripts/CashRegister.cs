using System.Collections.Generic;
using UnityEngine;

public class CashRegister : MonoBehaviour
{
    private Queue<CustomerAI> customerQueue = new Queue<CustomerAI>();
    
    [SerializeField] private Transform queuePosition;
    [SerializeField] private float queueSpacing = 0.8f;
    
    private bool isServing = false;
    private float serveCooldown = 1.5f;
    private float lastServeTime = 0f;

    public void RegisterCustomer(CustomerAI customer)
    {
        if (!customerQueue.Contains(customer))
        {
            customerQueue.Enqueue(customer);
            UpdateQueuePositions();
            Debug.Log($"Customer added. Queue size: {customerQueue.Count}");
        }
    }

    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (CustomerAI customer in customerQueue)
        {
            if (queuePosition != null)
            {
                Vector3 targetPos = queuePosition.position + new Vector3(0, -index * queueSpacing, 0);
                customer.MoveTo(targetPos);
            }
            index++;
        }
    }

    public bool TryServeNextCustomer()
    {
        if (Time.time < lastServeTime + serveCooldown)
            return false;

        if (customerQueue.Count == 0 || isServing)
            return false;

        isServing = true;
        CustomerAI customer = customerQueue.Dequeue();
        
        UpdateQueuePositions();

        customer.OnPaymentDone();

        lastServeTime = Time.time;

        Invoke(nameof(FinishServing), 0.3f);

        Debug.Log($"Customer served. Queue size: {customerQueue.Count}");
        return true;
    }

    private void FinishServing()
    {
        isServing = false;
    }

    public bool HasCustomers()
    {
        return customerQueue.Count > 0;
    }

    public int GetQueueCount()
    {
        return customerQueue.Count;
    }
}