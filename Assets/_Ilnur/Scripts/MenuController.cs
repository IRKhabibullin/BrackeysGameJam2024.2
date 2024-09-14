using System;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MovementController character;
    [SerializeField] private WaveController waveController;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject gameUI;
    
    public void StartGame()
    {
        character.ResetPosition();
        character.SetMoveSpeed(1);
        waveController.StartWaves();
        // delete all items from spawner

        menuUI.SetActive(false);
        gameUI.SetActive(true);
    }

    private void GameOver()
    {
        menuUI.SetActive(true);
        gameUI.SetActive(false);
        character.SetMoveSpeed(0);
    }

    private void Start()
    {
        character.SetMoveSpeed(0);
        waveController.StopWaves();
    }

    private void OnEnable()
    {
        MovementController.OnWaterTouched += GameOver;
    }

    private void OnDisable()
    {
        MovementController.OnWaterTouched -= GameOver;
    }
}
