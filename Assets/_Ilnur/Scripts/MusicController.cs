using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private MenuController menuController;
    
    [SerializeField] private AudioClip menuSoundtrack;
    [SerializeField] private AudioClip gameSoundtrack;
    [SerializeField] private AudioClip winSoundtrack;
    [SerializeField] private AudioClip loseSoundtrack;

    private AudioSource _audioSource;

    public void PlayGameSoundtrack()
    {
        _audioSource.loop = false;
        _audioSource.clip = gameSoundtrack;
        _audioSource.Play();
    }

    public void PlayMenuSoundtrack()
    {
        _audioSource.loop = true;
        _audioSource.clip = menuSoundtrack;
        _audioSource.Play();
    }

    private void GameOver()
    {
        StartCoroutine(FinishGameCoroutine(loseSoundtrack));
    }

    private void Victory()
    {
        StartCoroutine(FinishGameCoroutine(winSoundtrack));
    }

    private IEnumerator FinishGameCoroutine(AudioClip soundtrack)
    {
        for (var t = 1f; t > 0; t -= Time.deltaTime)
        {
            _audioSource.volume = t;
            yield return null;
        }

        _audioSource.Stop();
        yield return new WaitForSeconds(1);
        _audioSource.clip = soundtrack;
        _audioSource.Play();
        
        for (var t = 0f; t < 1; t += Time.deltaTime)
        {
            _audioSource.volume = t;
            yield return null;
        }

        while (_audioSource.isPlaying)
        {
            yield return null;
        }

        StartCoroutine(menuController.FadeInMenuUICoroutine());
        
        PlayMenuSoundtrack();
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
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
