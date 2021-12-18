using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;

    void Start() {
        this.slider.minValue = 0f;
    }

    public void SetMaxValue(float health) {
        this.slider.maxValue = health;
        this.slider.value = health;
    }

    public void SetValue(float health) {
        this.slider.value = health;
    }
}
