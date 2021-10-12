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

    [Header("Exploding")]
    public float timeToExplode = 60f * 2f;  // 60s * 2 => 2min
    private int lastInteractedFrame = -1;

    void Start()
    {

    }

    void Update()
    {
        if (!this.isDefused)
        {
            this.CheckDefusing();
            this.CheckDefused();
            this.CheckTicking();
        }
    }

    public void Interact()
    {
        if (!this.isDefused)
        {
            if (!this.isDefusing) this.StartDefusing();
            // If it is already defusing
            else this.lastInteractedFrame = Time.frameCount;
        }
    }

    public InteractType GetInteractType() {
        return InteractType.Bomb;
    }

    private void CheckDefusing()
    {
        if (this.isDefusing && this.lastInteractedFrame + 1 == Time.frameCount) this.ContinueDefusing();
        else this.ResetDefusing();
    }

    private void CheckDefused()
    {
        if (this.isDefusing && this.currentDefuseTime >= this.defuseTime) this.isDefused = true;
    }

    private void ResetDefusing()
    {
        this.isDefusing = false;
        this.currentDefuseTime += 0f;
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

    private void CheckTicking(){
        this.currentTickTime += Time.deltaTime;
        if (this.currentTickTime >= this.timeToExplode) {
            Debug.Log("EXPLODED");
        }
    }
}
