using System.Collections.Generic;
using UnityEngine;

public class CashRegister : MonoBehaviour
{
    private Queue<CustomerAI> customerQueue = new Queue<CustomerAI>();
    private Dictionary<CustomerAI, int> customerAmounts = new Dictionary<CustomerAI, int>();

    [SerializeField] private Transform queuePosition;
    [SerializeField] private float queueSpacing = 0.8f;
    [SerializeField] private float serveCooldown = 1.5f;
    private bool isServing = false;
    private float lastServeTime = 0f;

    private Dictionary<ItemType, int> prices = new Dictionary<ItemType, int>();

    private void Awake()
    {
        prices[ItemType.Banana] = 1;
        prices[ItemType.Corn] = 3;
        prices[ItemType.Egg] = 3;
        prices[ItemType.Milk] = 15;
        prices[ItemType.Popcorn] = 25;
        prices[ItemType.Yogurt] = 25;
    }

    public void RegisterCustomer(CustomerAI customer, List<ItemType> items)
    {
        if (customerQueue.Contains(customer)) return;

        // Рассчитываем сумму покупки
        int total = 0;
        foreach (ItemType type in items)
        {
            if (prices.ContainsKey(type))
                total += prices[type];
            else
                Debug.LogWarning($"Цена для {type} не установлена в кассе!");
        }

        customerQueue.Enqueue(customer);
        customerAmounts[customer] = total;
        UpdateQueuePositions();
        Debug.Log($"Покупатель встал в очередь. Сумма: {total}, очередь: {customerQueue.Count}");
    }

    public bool TryServeNextCustomer()
    {
        if (Time.time < lastServeTime + serveCooldown)
            return false;
        if (customerQueue.Count == 0 || isServing)
            return false;

        isServing = true;
        CustomerAI customer = customerQueue.Dequeue();
        int amount = customerAmounts[customer];
        customerAmounts.Remove(customer);

        PlayerWallet wallet = FindObjectOfType<PlayerWallet>();
        if (wallet != null)
            wallet.AddMoney(amount);
        else
            Debug.LogError("PlayerWallet не найден на сцене!");

        customer.OnPaymentDone();

        UpdateQueuePositions();
        lastServeTime = Time.time;
        Invoke(nameof(FinishServing), 0.3f);

        Debug.Log($"Обслужен покупатель на {amount} монет. В очереди: {customerQueue.Count}");
        return true;
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

    private void FinishServing()
    {
        isServing = false;
    }

    public bool HasCustomers() => customerQueue.Count > 0;
    public int GetQueueCount() => customerQueue.Count;
}