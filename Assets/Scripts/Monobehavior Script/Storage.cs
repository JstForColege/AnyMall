using System;

public class Storage
{
	private int _capacity;
	private int _currentAmount;
	private ResourceFood _item;

	public int Capacity
	{
		get => _capacity;
		set => _capacity = value;
	}
	public int CurrentAmount
	{
		get => _currentAmount;
		set => _currentAmount = value;
	}
	public ResourceFood Item
	{
		get => _item;
		set => _item = value;
	}

<<<<<<< HEAD
	public void AddItem()
	{
		if (this.Item == Item)
		{
			++CurrentAmount;
		}
	}
	public void RemoveItem() { }
}
=======
	public void AddItem() { }
	public void RemoveItem() { }*/
}
>>>>>>> 32b22e18101a3de36176176cf39aabf0f29dab90
