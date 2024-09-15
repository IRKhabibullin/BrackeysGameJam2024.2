using System;
using System.Collections;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MovementController character;
    [SerializeField] private WaveController waveController;
    [SerializeField] private AudioSource musicController;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject fadeScreen;

    [SerializeField] private AudioClip menuSoundtrack;
    [SerializeField] private AudioClip gameSoundtrack;
    
    public void StartGame()
    {
        character.ResetPosition();
        character.SetMoveSpeed(1);
        waveController.StartWaves();
        menuUI.SetActive(false);
        gameUI.SetActive(true);
        musicController.clip = gameSoundtrack;
        musicController.Play();
        // delete all items from spawner
    }

    public void PlayButtonPressed()
    {
        StartCoroutine(FadeCoroutine(StartGame));
    }

    private void GameOver()
    {
        StartCoroutine(FadeCoroutine(ShowMenu));
    }

    private void ShowMenu()
    {
        menuUI.SetActive(true);
        gameUI.SetActive(false);
        musicController.clip = menuSoundtrack;
        musicController.Play();
    }

    private IEnumerator FadeCoroutine(Action callback)
    {
        fadeScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        callback.Invoke();
        yield return new WaitForSeconds(1.5f);
        fadeScreen.SetActive(false);
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
