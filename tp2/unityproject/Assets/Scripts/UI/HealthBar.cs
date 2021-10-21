using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    void Start() {
        this.slider.minValue = 0f;
    }

    public void SetMaxHealth(float health) {
        this.slider.maxValue = health;
        this.slider.value = health;
    }

    public void SetHealth(float health) {
        this.slider.value = health;
    }
}
