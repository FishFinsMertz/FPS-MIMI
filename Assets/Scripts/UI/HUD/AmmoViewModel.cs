using System;
using UnityEngine;

public class AmmoViewModel: ViewModel
{
    public event Action<int> OnMagCountChanged;
    public event Action<int> OnAmmoCountChanged;

    private int magCount;
    private int ammoCount;

    public void SetMagCount(int value)
    {
        Debug.Log("SetMagCount: " + value);
        if (magCount != value)
        {
            magCount = value;
            OnMagCountChanged?.Invoke(magCount);
            Debug.Log("Mag Event Invoked");
        }
    }

    public void SetAmmoCount(int value)
    {
        Debug.Log("SetAmmoCount: " + value);
        if (ammoCount != value)
        {
            ammoCount = value;
            OnAmmoCountChanged?.Invoke(ammoCount);
            Debug.Log("Ammo Event Invoked");
        }
    }

    public int GetMagCount() => magCount;
    public int GetAmmoCount() => ammoCount;
}
