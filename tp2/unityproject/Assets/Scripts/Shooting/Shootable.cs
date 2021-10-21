using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public int currentHealth = 100;

    public bool showInUI = false;
    public HealthBar healthBar;

    private bool isDead = false;
    private Animator characterAnimator;
    public ParticleSystem bloodParticles;

    void Start() {
        this.characterAnimator = GetComponent<Animator>();
        // In case we show health in UI
        if (this.showInUI) {
            this.healthBar.SetMaxHealth(this.currentHealth);
        }
    }

    public void ApplyDamage(int amount) {
        if (!this.isDead) {
            // Remove damage amount from health
            this.currentHealth -= amount;
            this.bloodParticles.Play();
            // Check if alter UI
            if (this.showInUI) {
                this.healthBar.SetHealth(this.currentHealth);
            }
            // Check if health has fallen below zero
            if (this.currentHealth <= 0) 
            {
                this.isDead = true;
                this.SetDeath();
            }
        }
    }

    private void SetDeath() {
        if (this.characterAnimator != null) this.characterAnimator.SetBool("Death_b", true);
    }
}
