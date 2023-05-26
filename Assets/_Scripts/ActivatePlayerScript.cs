using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePlayerScript : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<PlayerScript>().enabled = true; //so the player cant move while the falling animations are running
    }
}
