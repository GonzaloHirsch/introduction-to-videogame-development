using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public Text timerText;

    private float timerLimit;
    private float initialTimerLimit;
    private TimeSpan timeSpan;
    private string timeText;
    private bool isCounting = true;

    void Update()
    {
        if (this.isCounting)
        {
            this.timerLimit -= Time.deltaTime;
            this.UpdateTimerText();
        }
    }

    public void SetTimerLimit(float limit)
    {
        this.timerLimit = limit;
        this.initialTimerLimit = limit;
    }

    private void UpdateTimerText()
    {
        // Set text
        this.timeSpan = TimeSpan.FromSeconds(this.timerLimit);
        this.timeText = string.Format("{0:D1}m {1:D2}s", timeSpan.Minutes, timeSpan.Seconds);
        this.timerText.text = timeText;
        // Set text color
        this.timerText.color = this.GetTextColor();
    }

    private Color GetTextColor() {
        float gb = this.timerLimit / this.initialTimerLimit;
        return new Color(1, gb, gb);
    }

    public void StopTimer()
    {
        this.isCounting = false;
    }
}
