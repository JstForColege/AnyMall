using System;

public class Producer
{
    private Resource _ingridient;
    private int _maxIngridient;

    private Resource _outgridient;
    private int _maxOutgridient;

    private int _makingTime;
    private bool _isWorking = true;

    public Resource Ingridient
    {
        get => _ingridient;
        set => _ingridient = value;
    }
    public int MaxIngridient
    {
        get => _maxIngridient;
        set => _maxIngridient = value;
    }

    public Resource Outgridient
    {
        get => _outgridient;
        set => _outgridient = value;
    }
    public int MaxOutgridient
    {
        get => _maxOutgridient;
        set => _maxOutgridient = value;
    }

    public int MakingTime
    {
        get => _makingTime;
        set => _makingTime = value;
    }
    public bool IsWorking
    {
        get => _isWorking;
        set => _isWorking = value;
    }
}
