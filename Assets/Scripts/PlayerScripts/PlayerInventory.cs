using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInventory
{
    [SerializeField] private int maxSize = 3;
    private Stack<ItemData> stack = new Stack<ItemData>();

    public PlayerInventory(int initialMaxSize = 1)
    {
        maxSize = initialMaxSize;
    }

    public int Count => stack.Count;
    public int MaxSize => maxSize;
    public bool IsFull => stack.Count >= maxSize;
    public bool IsEmpty => stack.Count == 0;

    public bool Push(ItemData item)
    {
        if (IsFull)
        {
            Debug.Log("Инвентарь полон");
            return false;
        }
        stack.Push(item);
        Debug.Log($"Предмет {item.Name} добавлен в инвентарь. Всего: {stack.Count}");
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
    public string Id;
    public string Name;
    public Sprite Icon;

    public ItemData(string id, string name, Sprite icon = null)
    {
        Id = id;
        Name = name;
        //Icon = icon;
    }
}