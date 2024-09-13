using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    [SerializeField] private float timeTillBored;
    
    private bool _isLookingAround;
    private float _idleTime;
    private bool _isBored;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_isBored)
        {
            _idleTime += Time.deltaTime;

            if (_idleTime > timeTillBored && stateInfo.normalizedTime % 1 < 0.02f)
            {
                _isBored = true;
                animator.SetFloat("isBored", 1);
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.98)
        {
            _isBored = false;
            _idleTime = 0;
            animator.SetFloat("isBored", 0);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
