using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;

    public void Initialize(ItemData item)
    {
        itemData = item;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && item.Icon != null)
            sr.sprite = item.Icon;
    }

    public bool PickUp(PlayerInventoryHandler inventoryHandler)
    {
        if (inventoryHandler == null || itemData == null) return false;
        if (inventoryHandler.TryAddItem(itemData))
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}