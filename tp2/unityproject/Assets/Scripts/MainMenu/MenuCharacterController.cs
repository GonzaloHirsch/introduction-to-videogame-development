using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCharacterController : MonoBehaviour
{
    public int gunType = 9;
    private Animator characterAnimator;

    void Start()
    {
        this.characterAnimator = GetComponent<Animator>();
        this.characterAnimator.SetInteger("WeaponType_int", this.gunType);
    }
}
