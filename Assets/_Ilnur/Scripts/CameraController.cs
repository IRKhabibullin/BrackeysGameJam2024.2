using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerObject;

    private Vector3 _basePosition;

    private void Awake()
    {
        _basePosition = transform.position;
    }

    void Update()
    {
        transform.position = _basePosition + playerObject.position;
    }
}
