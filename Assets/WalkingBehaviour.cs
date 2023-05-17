using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingBehaviour : StateMachineBehaviour
{
    [SerializeField] private int _numberOfWalkingAnims;
    private int _walkingAnimation;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _walkingAnimation = Random.Range(1, _numberOfWalkingAnims + 1);     
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("WalkingAnimation", _walkingAnimation, .2f, Time.deltaTime);
    }

}
