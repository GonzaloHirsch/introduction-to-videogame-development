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
        Debug.Log("DEFUSING-" + this.currentDefuseTime);
    }

    private void StartDefusing()
    {
        this.currentDefuseTime = 0f;
        this.isDefusing = true;
        this.lastInteractedFrame = Time.frameCount;
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
}
