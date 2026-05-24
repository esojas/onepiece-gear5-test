using UnityEngine;

public class OnFinishEnemyAnimation : StateMachineBehaviour
{
    [SerializeField] private string animation;


    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(stateInfo.length.ToString());
        animator.GetComponent<AnimationScript>().ChangeAnimation(animation, .01f, stateInfo.length);
    }
}
