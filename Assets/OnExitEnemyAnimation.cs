using UnityEngine;

public class OnExitEnemyAnimation : StateMachineBehaviour
{
    [SerializeField] private string animation;
    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<AnimationScript>().ChangeAnimation(animation, 0.2f);
    }


}
