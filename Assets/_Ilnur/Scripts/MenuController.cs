using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MovementController character;
    [SerializeField] private WaveController waveController;
    [SerializeField] private GameObject menuUI;
    
    public void StartGame()
    {
        character.ResetPosition();
        character.SetMoveSpeed(1);
        waveController.StartWaves();
        // delete all items from spawner

        menuUI.SetActive(false);
    }

    private void GameOver()
    {
        menuUI.SetActive(true);
        character.SetMoveSpeed(0);
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
