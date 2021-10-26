using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public int currentHealth = 100;

    public bool showInUI = false;
    public ProgressBar healthBar;

    private bool isDead = false;
    private Animator characterAnimator;
    public ParticleSystem bloodParticles;
    public bool emitPlayerDeath = false;
    public bool emitEnemyDeath = false;

    void Awake() {
        this.bloodParticles.Stop();
        this.characterAnimator = GetComponent<Animator>();
        // In case we show health in UI
        if (this.showInUI) {
            this.healthBar.SetMaxValue(this.currentHealth);
        }
    }

    public void ApplyDamage(int amount) {
        if (!this.isDead) {
            // Remove damage amount from health
            this.currentHealth -= amount;
            this.bloodParticles.Play();
            // Check if alter UI
            if (this.showInUI) {
                this.healthBar.SetValue(this.currentHealth);
            }
            // Check if health has fallen below zero
            if (this.currentHealth <= 0) 
            {
                // Play animation before marking to avoid race condition errors
                this.SetDeath();
                this.isDead = true;
            }
        }
    }

    private void SetDeath() {
        if (this.characterAnimator != null) this.characterAnimator.SetBool("Death_b", true);
        if (this.emitPlayerDeath) FrameLord.GameEventDispatcher.Instance.Dispatch(this, EvnPlayerDeath.notifier);
        if (this.emitEnemyDeath) FrameLord.GameEventDispatcher.Instance.Dispatch(this, EvnEnemyDeath.notifier);
    }

    public bool IsDead() {
        return this.isDead;
    }
}
