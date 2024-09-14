using System.Collections;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [SerializeField] private ParticleSystem rain;
    [SerializeField] private ParticleSystem lightning;
    [SerializeField] private Transform player;

    private WaitForSeconds _waitOneSecond = new(1);

    private void StartWeather(object sender, WaveData waveData)
    {
        switch (waveData.weather)
        {
            case WeatherEffect.Rain:
                rain.Play();
                rain.GetComponent<AudioSource>().Play();
                break;
            case WeatherEffect.Clouds:
                // darken lights, start moving clouds
                break;
            case WeatherEffect.Thunder:
                StartCoroutine(StartThunderCoroutine());
                break;
            case WeatherEffect.Lightning:
                StartCoroutine(StartLightningCoroutine());
                break;
        }
    }

    private void ResetWeather()
    {
        rain.Stop();
        rain.GetComponent<AudioSource>().Stop();
        StopAllCoroutines();
    }

    private IEnumerator StartThunderCoroutine()
    {
        yield break;
    }

    private IEnumerator StartLightningCoroutine()
    {
        while (true)
        {
            if (Random.Range(0, 7) > 5)
            {
                lightning.Play();
                lightning.GetComponent<AudioSource>().Play();
                yield return _waitOneSecond;
            }
            yield return _waitOneSecond;
        }
    }

    private void OnEnable()
    {
        WaveController.OnWaveUp += StartWeather;
    }

    private void OnDisable()
    {
        WaveController.OnWaveUp -= StartWeather;
    }
}

public enum WeatherEffect
{
    None,
    Rain,
    Clouds,
    Thunder,
    Lightning
}
