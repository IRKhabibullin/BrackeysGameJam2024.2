using System;
using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private WaveValues_SO waveValues;
    [SerializeField] private WaterLevels_SO waterLevels;
    private IEnumerator _rippleCoroutine;
    [SerializeField] private MeshRenderer waterMesh;
    [SerializeField] private LayerMask beachLayer;
    [SerializeField] private AudioSource wavesAudioSource;

    public static event EventHandler <WaveData> OnWaveUp;

    void Awake()
    {
        GenerateWaterLevels();
        StartWaves();
    }

    private void GenerateWaterLevels()
    {
        foreach (var waterLevel in waterLevels.waterLevels)
        {
            var waveMiddle = (waterLevel.upperBorder - waterLevel.lowerBorder) / 2;
            var boxCenter = new Vector3(0, 5, waterLevel.lowerBorder + waveMiddle);
            var halfExtents = new Vector3(waterMesh.bounds.size.x / 2, 0.01f, waveMiddle);
            if (Physics.BoxCast(boxCenter, halfExtents, Vector3.down, out var raycastHit, Quaternion.identity, 10, beachLayer))
            {
                waterLevel.highTideY = raycastHit.point.y;
            }
        }
    }

    private IEnumerator RippleCoroutine()
    {
        foreach (var waveData in waveValues.waves)
        {
            wavesAudioSource.clip = waveData.waveSound;
            wavesAudioSource.volume = 0;
            yield return MoveWaveCoroutine(waterLevels.GetWaterLavel(waveData.lowTideLayer).highTideY, waveData.lowTideMoveTime);
            yield return new WaitForSeconds(waveData.lowTideDuration);
            yield return MoveWaveCoroutine(waterLevels.GetWaterLavel(waveData.highTideLayer).highTideY, waveData.highTideMoveTime);
            OnWaveUp?.Invoke(this, waveData);
            yield return new WaitForSeconds(waveData.highTideDuration);
        }
    }

    private IEnumerator MoveWaveCoroutine(float newWaveHeight, float waveMoveTime)
    {
        wavesAudioSource.Play();
        var currentPosition = transform.position;
        for(var f = 0f; f <= waveMoveTime; f += Time.deltaTime) {
            var t = f / waveMoveTime;
            t = t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
            transform.position = new Vector3(0, Mathf.Lerp(currentPosition.y, newWaveHeight, t), 0);

            if (t < 0.2f)
            {
                wavesAudioSource.volume = t;
            }
            else if (t > 0.8f)
            {
                wavesAudioSource.volume = 1 - t;
            }
            
            yield return null;
        }
        wavesAudioSource.Stop();
    }

    private IEnumerator PlayWaveSoundCoroutine(WaveData wave)
    {
        wavesAudioSource.Play();
        var volumeChangeTime = wave.lowTideMoveTime * 0.2f;
        while (wavesAudioSource.volume < 1)
        {
            wavesAudioSource.volume += Time.deltaTime / volumeChangeTime;
            yield return null;
        }

        yield return new WaitForSeconds(wave.lowTideMoveTime * 0.6f);
        while (wavesAudioSource.volume > 0)
        {
            wavesAudioSource.volume -= Time.deltaTime / volumeChangeTime;
            yield return null;
        }
        wavesAudioSource.Stop();
    }

    public void StopWaves()
    {
        transform.position = new Vector3(0, -0.6f, 0);
        if (_rippleCoroutine != null)
            StopCoroutine(_rippleCoroutine);
    }

    public void StartWaves()
    {
        if (_rippleCoroutine != null)
            StopCoroutine(_rippleCoroutine);
        _rippleCoroutine = RippleCoroutine();
        StartCoroutine(_rippleCoroutine);
    }
    
    private void OnEnable()
    {
        MovementController.OnWaterTouched += StopWaves;
    }

    private void OnDisable()
    {
        MovementController.OnWaterTouched -= StopWaves;
    }
}
