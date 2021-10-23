using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpiringEffect : MonoBehaviour
{
    void Start() {
        ParticleSystem exp = GetComponent<ParticleSystem>();
        exp.Play();
        Destroy(this.gameObject, exp.main.duration);
    }
}
