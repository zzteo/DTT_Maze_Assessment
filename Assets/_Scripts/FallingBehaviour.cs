using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBehaviour : StateMachineBehaviour
{
    [SerializeField] private ParticleSystem _dust;
    private ParticleSystem _particles;
    private GameObject _player;

    //Instantiates dust particles when the player hits the ground at the beginning of a level
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _particles = Instantiate(_dust, _player.transform.position, Quaternion.identity);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(_particles.gameObject, 5);
    }
}
