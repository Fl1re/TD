using System;

public class EconomyManager
{
    private int _resources = 1000;
    public event Action<int> OnResourcesChanged;

    public int Resources => _resources;

    public void AddResources(int amount)
    {
        _resources += amount;
        OnResourcesChanged?.Invoke(_resources);
    }

    public bool SpendResources(int amount)
    {
        if (_resources >= amount)
        {
            _resources -= amount;
            OnResourcesChanged?.Invoke(_resources);
            return true;
        }
        return false;
    }
}
