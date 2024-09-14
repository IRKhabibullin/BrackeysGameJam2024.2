using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSlow : MonoBehaviour
{
    [SerializeField] float slowPercent;
    private MovementController movementController;
    private Animator playerAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (slowPercent == 0)
            {
                float stuckTime = 2f;
                movementController.SetMoveSpeed(slowPercent, stuckTime);
                playerAnimator = movementController.GetAnimator();
                playerAnimator.SetBool("isStuck", true);
                StartCoroutine(ChangeBoolWithDelay(stuckTime));
            }
            else
            {
                movementController.SetMoveSpeed(slowPercent);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //this is unnccessary if we hot object which staggers, but whatever
            movementController.SetMoveSpeed(1); // setting speed to base
        }
    }

    public void SetPlayer(MovementController movementController)
    {
        this.movementController = movementController;
    }

    // Coroutine to introduce a 1-second delay
    private IEnumerator ChangeBoolWithDelay(float time)
    {
        yield return new WaitForSeconds(time);
        playerAnimator.SetBool("isStuck", false);
        movementController.SetMoveSpeed(1);
    }
}
