using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSlow : MonoBehaviour
{
    [SerializeField] float slowPercent;
    private MovementController movementController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            movementController.SetMoveSpeed(slowPercent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            movementController.SetMoveSpeed(1); // setting speed to base
        }
    }

    public void SetPlayer(MovementController movementController)
    {
        this.movementController = movementController;
    }
}
