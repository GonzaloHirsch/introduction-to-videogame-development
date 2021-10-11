using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public int currentHealth = 100;

    private bool isDead = false;
    private Animator characterAnimator;
    public ParticleSystem bloodParticles;

    void Start() {
        this.characterAnimator = GetComponent<Animator>();
    }

    public void ApplyDamage(int amount) {
        if (!this.isDead) {
            // Remove damage amount from health
            this.currentHealth -= amount;
            this.bloodParticles.Play();
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
