using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingBehaviour : StateMachineBehaviour
{
    [SerializeField] private int _numberOfWalkingAnims;
    private int _walkingAnimation;

    //When in the walking state, it randomly changes through multiple walking animations gradually 
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _walkingAnimation = Random.Range(1, _numberOfWalkingAnims + 1);     
    }
   
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("WalkingAnimation", _walkingAnimation, .2f, Time.deltaTime);
    }
}
