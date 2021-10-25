using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IInteractable
{
    [Header("Defusing")]
    public float defuseTime = 10f;
    private float currentDefuseTime = 0f;
    private float currentTickTime = 0f;
    private bool isDefusing = false;
    private bool isDefused = false;
    private bool isExploded = false;

    [Header("Exploding")]
    public float timeToExplode = 60f * 2f;  // 60s * 2 => 2min
    private int lastInteractedFrame = -1;

    [Header("UI")]
    public bool showInUI = false;
    public ProgressBar defuseBar;
    public Outline outline;

    void Start() {
        this.outline = GetComponent<Outline>();
        if (this.outline) this.outline.OutlineColor = new Color(1f, 0f, 0f);
        this.ResetProgressBar();
    }

    void Update()
    {
        if (!this.isDefused && !this.isExploded)
        {
            this.CheckDefusing();
            this.CheckDefused();
            this.CheckTicking();
        }
    }

    public void Interact()
    {
        if (!this.isDefused && !this.isExploded)
        {
            if (!this.isDefusing)
            {
                this.StartDefusing();
            }
            else
            {
                // If it is already defusing
                this.lastInteractedFrame = Time.frameCount;
            }
        }
    }

    public InteractType GetInteractType()
    {
        return InteractType.Bomb;
    }

    private void CheckDefusing()
    {
        if (this.isDefusing && (this.lastInteractedFrame + 1 == Time.frameCount || this.lastInteractedFrame == Time.frameCount))
        {
            this.ContinueDefusing();
        }
        else
        {
            this.ResetDefusing();
        }
    }

    private void CheckDefused()
    {
        if (this.isDefusing && this.currentDefuseTime >= this.defuseTime)
        {
            this.isDefused = true;
            FrameLord.GameEventDispatcher.Instance.Dispatch(this, EvnBombDefuse.notifier);
            // Green outline
            if (this.outline) this.outline.OutlineColor = new Color(0f, 1f, 0f);
            if (this.showInUI) {
                this.defuseBar.SetValue(0f);
            }
        }
    }

    private void ResetDefusing()
    {
        this.isDefusing = false;
        this.currentDefuseTime = 0f;
        this.lastInteractedFrame = -1;
    }

    private void ContinueDefusing()
    {
        this.currentDefuseTime += Time.deltaTime;
        if (this.showInUI) {
            this.defuseBar.SetValue(this.currentDefuseTime);
        }
    }

    private void StartDefusing()
    {
        this.currentDefuseTime = 0f;
        this.isDefusing = true;
        this.lastInteractedFrame = Time.frameCount;
        this.ResetProgressBar();
    }

    private void CheckTicking()
    {
        this.currentTickTime += Time.deltaTime;
        if (this.currentTickTime >= this.timeToExplode)
        {
            this.isExploded = true;
            FrameLord.GameEventDispatcher.Instance.Dispatch(this, EvnBombExplode.notifier);
        }
    }

    public void SetTimeToExplode(float time) {
        this.timeToExplode = time;
    }

    public bool IsDefused() {
        return this.isDefused;
    }

    public void ResetProgressBar() {
        if (this.showInUI) {
            this.defuseBar.SetMaxValue(this.defuseTime);
            this.defuseBar.SetValue(0f);
        }
    }
}
