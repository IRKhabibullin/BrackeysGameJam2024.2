using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

public class WeatherController : MonoBehaviour
{
    [SerializeField] private ParticleSystem rain;
    [SerializeField] private ParticleSystem lightning;
    [SerializeField] private Transform clouds;
    [SerializeField] private Transform player;
    [SerializeField] private Light globalLight;
    [SerializeField] private Light thunderLight;
    [SerializeField] private Light lightningLight;
    
    [SerializeField] private Color clearWeatherColor;
    [SerializeField] private Color cloudsWeatherColor;
    [SerializeField] private Color rainWeatherColor;
    [SerializeField] private Color thunderWeatherColor;
    [SerializeField] private Color lightningWeatherColor;

    private WaitForSeconds _waitOneSecond = new(1);

    private void ChangeWeather(object sender, WaveData waveData)
    {
        switch (waveData.weather)
        {
            case WeatherEffect.Rain:
                StartCoroutine(SetLighting(rainWeatherColor));
                rain.Play();
                rain.GetComponent<AudioSource>().Play();
                break;
            case WeatherEffect.Clouds:
                StartCoroutine(SetLighting(cloudsWeatherColor));
                StartCoroutine(StartCloudsCoroutine());
                // darken lights, start moving clouds
                break;
            case WeatherEffect.Thunder:
                StartCoroutine(SetLighting(thunderWeatherColor));
                StartCoroutine(StartThunderCoroutine());
                break;
            case WeatherEffect.Lightning:
                StartCoroutine(SetLighting(lightningWeatherColor));
                StartCoroutine(StartLightningCoroutine());
                break;
        }
    }

    private void ResetWeather()
    {
        globalLight.color = clearWeatherColor;
        clouds.gameObject.SetActive(false);
        clouds.rotation = Quaternion.Euler(0, -22, 0);
        rain.Stop();
        rain.GetComponent<AudioSource>().Stop();
        StopAllCoroutines();
    }

    private IEnumerator SetLighting(Color newColor)
    {
        var oldColor = globalLight.color;
        for (var t = 0f; t < 1; t += Time.deltaTime)
        {
            globalLight.color = Color.Lerp(oldColor, newColor, t);
            yield return null;
        }
    }

    private IEnumerator StartCloudsCoroutine()
    {
        clouds.gameObject.SetActive(true);
        while (true)
        {
            clouds.transform.Rotate(0, -0.01f, 0);
            yield return null;
        }
    }

    private IEnumerator StartThunderCoroutine()
    {
        while (true)
        {
            if (Random.Range(0, 5) > 3)
            {
                thunderLight.enabled = true;
                yield return new WaitForSeconds(0.05f);
                thunderLight.enabled = false;
            }
            yield return _waitOneSecond;
        }
    }

    private IEnumerator StartLightningCoroutine()
    {
        while (true)
        {
            if (Random.Range(0, 7) > 5)
            {
                lightning.transform.position = player.position +
                                               new Vector3(Random.Range(-10, 10), Random.Range(-10, 10),
                                                   Random.Range(-10, 10));
                lightning.Play();
                lightning.GetComponent<AudioSource>().Play();
                lightningLight.enabled = true;
                yield return new WaitForSeconds(0.05f);
                lightningLight.enabled = false;
                yield return _waitOneSecond;
            }
            yield return _waitOneSecond;
        }
    }

    private void OnEnable()
    {
        WaveController.OnWaveUp += ChangeWeather;
        MovementController.OnWaterTouched += ResetWeather;
    }

    private void OnDisable()
    {
        WaveController.OnWaveUp -= ChangeWeather;
        MovementController.OnWaterTouched -= ResetWeather;
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
