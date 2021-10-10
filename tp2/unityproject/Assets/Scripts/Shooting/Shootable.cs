using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public int currentHealth = 100;

    public void ApplyDamage(int amount) {
        // Remove damage amount from health
        this.currentHealth -= amount;
        // Check if health has fallen below zero
        if (this.currentHealth <= 0) 
        {
            // If health has fallen below zero, deactivate it 
            gameObject.SetActive (false);
        }
    }
}
