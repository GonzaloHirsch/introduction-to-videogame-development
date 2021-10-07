using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public int life = 100;

    void ApplyDamage(int amount) {
        this.life -= amount;
    }
}
