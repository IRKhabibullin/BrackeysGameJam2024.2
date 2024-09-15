using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MovementController character;
    [SerializeField] private WaveController waveController;
    [SerializeField] private MusicController musicController;
    [SerializeField] private Image menuBackground;
    [SerializeField] private CanvasGroup menuUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject fadeScreen;
    
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;
    
    public void StartGame()
    {
        character.ResetPosition();
        character.SetMoveSpeed(1);
        waveController.StartWaves();
        menuBackground.gameObject.SetActive(false);
        menuUI.alpha = 0;
        gameUI.SetActive(true);
        musicController.PlayGameSoundtrack();
        // todo delete all items from spawner
    }

    public IEnumerator FadeInMenuUICoroutine()
    {
        for (var t = 0f; t < 1; t += Time.deltaTime)
        {
            menuUI.alpha = t;
            yield return null;
        }
    }

    public void PlayButtonPressed()
    {
        StartCoroutine(FadeCoroutine(StartGame));
    }

    private void GameOver()
    {
        menuBackground.sprite = loseSprite;
        StartCoroutine(FadeCoroutine(ShowMenu));
    }

    private void Victory()
    {
        menuBackground.sprite = winSprite;
        StartCoroutine(FadeCoroutine(ShowMenu));
    }

    private void ShowMenu()
    {
        menuUI.alpha = 0;
        menuBackground.gameObject.SetActive(true);
        gameUI.SetActive(false);
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

        musicController.PlayMenuSoundtrack();
    }

    private void OnEnable()
    {
        MovementController.OnWaterTouched += GameOver;
        WaveController.OnAllWavesEnded += Victory;
    }

    private void OnDisable()
    {
        MovementController.OnWaterTouched -= GameOver;
        WaveController.OnAllWavesEnded -= Victory;
    }
}
