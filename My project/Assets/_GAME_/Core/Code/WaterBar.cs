using UnityEngine;
using UnityEngine.UI;

public class WaterBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxWater(int maxWater)
    {
        slider.maxValue = maxWater;
        slider.value = maxWater;
    }

    public void SetWater(int water)
    {
        slider.value = water;
    }
}