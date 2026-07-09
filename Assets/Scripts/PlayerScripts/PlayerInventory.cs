using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInventory
{
    [SerializeField] private int maxSize = 1;
    private Stack<ItemData> stack = new Stack<ItemData>();

    public PlayerInventory(int initialMaxSize = 1)
    {
        maxSize = initialMaxSize;
    }

    public int Count => stack.Count;
    public int MaxSize => maxSize;
    public bool IsFull => stack.Count >= maxSize;
    public bool IsEmpty => stack.Count == 0;

    public void SetMaxSize(int newSize)
    {
        maxSize = newSize;
    }

    public bool Push(ItemData item)
    {
        if (IsFull)
        {
            Debug.Log("Инвентарь полон");
            return false;
        }
        stack.Push(item);
        Debug.Log($"Предмет {item.Type.ToString()} добавлен в инвентарь. Всего: {stack.Count}");
        return true;
    }

    public ItemData Pop()
    {
        if (IsEmpty)
            return null;
        return stack.Pop();
    }

    public ItemData Peek()
    {
        if (IsEmpty)
            return null;
        return stack.Peek();
    }

    public void Clear()
    {
        stack.Clear();
    }
}

[System.Serializable]
public class ItemData //сделать enum
{
    public ItemType Type;
    public Sprite Icon;
    public ItemData(ItemType type, Sprite icon = null)
    {
        Type = type;
        Icon = icon;
    }
}
public enum ItemType
{
    Banana, Corn,
    Egg, Milk,
    Popcorn, Yogurt
}