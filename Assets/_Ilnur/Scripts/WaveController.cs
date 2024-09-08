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
            yield return MoveWaveCoroutine(lowWaveHeight);
            yield return new WaitForSeconds(waveData.lowerHeightTime);
            yield return MoveWaveCoroutine(highWaveHeight);
            yield return new WaitForSeconds(waveData.upperHeightTime);
        }
    }

    private IEnumerator MoveWaveCoroutine(float newWaveHeight)
    {
        var currentPosition = transform.position;
        var t = 0f;
        while (!Mathf.Approximately(currentPosition.y, newWaveHeight))
        {
            currentPosition = new Vector3(0, Mathf.Lerp(currentPosition.y, newWaveHeight, t), 0);
            transform.position = currentPosition;
                
            t += rippleSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
