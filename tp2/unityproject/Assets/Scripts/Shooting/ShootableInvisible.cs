using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableInvisible : MonoBehaviour
{
    public GameObject topLevelParent;
    public Shootable shootable;
    void Start()
    {
        this.shootable = GetComponentInParent<Shootable>();   
    }

    void OnBecameInvisible() {
        if (this.shootable.IsDead()) this.topLevelParent.SetActive(false);
    }
}
