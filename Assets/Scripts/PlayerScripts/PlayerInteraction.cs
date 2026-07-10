using Assets.Scripts.Monobehavior_Script;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PlayerInventoryHandler inventoryHandler;

    private void Start()
    {
        if (inventoryHandler == null)
            inventoryHandler = GetComponent<PlayerInventoryHandler>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out ResourceNode node))
        {
            if (inventoryHandler.Inventory.IsFull) return;
            ItemData item = node.Harvest();
            if (item != null)
            {
                inventoryHandler.TryAddItem(item);
            }
            return;
        }

        if (other.TryGetComponent(out Storage shelf))
        {
            if (inventoryHandler.Inventory.IsEmpty) return;
            ItemData item = inventoryHandler.GetFirstItem();
            if (shelf.AddItem(item))
            {
                inventoryHandler.RemoveTopItem();
            }
            return;
        }

        if (other.TryGetComponent(out ProducerInput input))
        {
            if (inventoryHandler.Inventory.IsEmpty) return;
            ItemData item = inventoryHandler.GetFirstItem();
            if (input.producer.AddItem(item))
            {
                inventoryHandler.RemoveTopItem();
            }
            return;
        }

        if (other.TryGetComponent(out ProducerOutput output))
        {
            if (inventoryHandler.Inventory.IsFull) return;
            ItemData item = output.producer.TakeOutgridient();
            if (item != null)
            {
                inventoryHandler.TryAddItem(item);
            }
            return;
        }

        if (other.TryGetComponent(out HooliganAI hooligan))
        {
            bool chased = hooligan.TryChaseAway();
            if (chased)
                Debug.Log("Player: Chased hooligan");
            else
                Debug.Log("Player: Hooligan not at shelf yet");
            return;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out CashRegister cashRegister))
        {
            bool served = cashRegister.TryServeNextCustomer();
            if (served)
                Debug.Log("Player: Served customer");
            return;
        }
    }
}