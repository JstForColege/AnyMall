using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Stack<ItemData> stack = new Stack<ItemData>();
    private int maxSize;
    public PlayerInventory(int initialMaxSize = 1)
    {
        maxSize = initialMaxSize;
    }

    public int Count { get { return stack.Count; } }
    public int MaxSize { get { return maxSize; } }
    public bool IsFull{ get 
        {
            if (stack.Count >= maxSize)
                return true;
            else
                return false;
        }
    }

    public bool IsEmpty { get
        {
            if (stack.Count == 0)
                return true;
            else
                return false;
        }
    }

    public void Push(ItemData item)
    {
        if (IsFull)
        {
            Debug.Log("Инвентарь полон");
            return;
        }
        stack.Push(item);
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

    public class ItemData
    {
        public string Id;
        public string Name;
    }
}