using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDView : MonoBehaviour
{
    public TextMeshProUGUI magCountText;
    public TextMeshProUGUI ammoCountText;
    public Slider healthSlider;

    public void Bind(ViewModel vm)
    {
        switch (vm)
        {
            case AmmoViewModel ammoVM:
                Debug.Log("AmmoViewModel Binded");
                ammoVM.OnMagCountChanged += val => magCountText.text = val.ToString();
                ammoVM.OnAmmoCountChanged += val => ammoCountText.text = val.ToString();
                break;

            case HealthViewModel healthVM:
                Debug.Log("HealthViewModel Binded");
                healthVM.OnHealthChanged += val => healthSlider.value = val;
                healthVM.OnMaxHealthChanged += val => healthSlider.maxValue = val;
                break;

            default:
                break;
        }
    }
}