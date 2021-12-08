using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public Animator animator;

    void start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player") 
        {
            print("Open door");
            animator.SetTrigger("openDoor");
        }
    }
}
