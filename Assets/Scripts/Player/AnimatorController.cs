using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour{
    public Animator animator;

    private PlayerController pc;

    void Start(){
        pc = GetComponent<PlayerController>();
    }

    void Update(){
        Vector2 dir = pc.GetDirection();

        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
    }
}
