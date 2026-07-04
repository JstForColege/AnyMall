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
	public void AddItem()
	{
		if (this.Item == Item)
		{
			++CurrentAmount;
		}
	}
	public void RemoveItem() { }
}

