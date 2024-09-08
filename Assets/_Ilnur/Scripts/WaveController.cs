using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [Range(-2, 0)] [SerializeField] private float lowWaveHeight;
    [Range(0, 2)] [SerializeField] private float highWaveHeight;

    [SerializeField] private WaveValues_SO waveValues;

    [SerializeField] private float ripplePeriod;
    [SerializeField] private float rippleSpeed;
    private IEnumerator _rippleCoroutine;
    private bool _lastDirection;

    void Start()
    {
        _rippleCoroutine = RippleCoroutine();
        StartCoroutine(_rippleCoroutine);
    }

    private IEnumerator RippleCoroutine()
    {
        foreach (var waveData in waveValues.waves)
        {
            yield return MoveWaveCoroutine(lowWaveHeight, waveData.lowTideTime);
            yield return new WaitForSeconds(waveData.highTideTimeout);
            yield return MoveWaveCoroutine(highWaveHeight, waveData.highTideTime);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator MoveWaveCoroutine(float newWaveHeight, float waveMoveTime)
    {
        var currentPosition = transform.position;
        for(float f = 0; f <= waveMoveTime; f += Time.deltaTime) {
            transform.position = new Vector3(0, Mathf.Lerp(currentPosition.y, newWaveHeight, f / waveMoveTime), 0);
            yield return null;
        }
    }
}
