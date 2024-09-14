using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MovementController character;
    [SerializeField] private WaveController waveController;
    [SerializeField] private AudioSource musicController;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject gameUI;

    [SerializeField] private AudioClip menuSoundtrack;
    [SerializeField] private AudioClip gameSoundtrack;
    
    public void StartGame()
    {
        character.ResetPosition();
        character.SetMoveSpeed(1);
        waveController.StartWaves();
        // delete all items from spawner

        menuUI.SetActive(false);
        gameUI.SetActive(true);

        musicController.clip = gameSoundtrack;
        musicController.Play();
    }

    private void GameOver()
    {
        menuUI.SetActive(true);
        gameUI.SetActive(false);
        character.SetMoveSpeed(0);

        musicController.clip = menuSoundtrack;
        musicController.Play();
    }

    private void Start()
    {
        character.SetMoveSpeed(0);
        waveController.StopWaves();

        musicController.clip = menuSoundtrack;
        musicController.Play();
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
