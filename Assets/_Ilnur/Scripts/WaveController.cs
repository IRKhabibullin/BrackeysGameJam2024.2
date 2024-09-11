using System;
using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private WaveValues_SO waveValues;
    [SerializeField] private WaterLevels_SO waterLevels;
    [SerializeField] private Transform beachPlane;

    [SerializeField] private float ripplePeriod;
    [SerializeField] private float rippleSpeed;
    private IEnumerator _rippleCoroutine;
    private bool _lastDirection;

    public static event EventHandler <WaveData> OnWaveUp;

    void Start()
    {
        GenerateWaterLevels();
        _rippleCoroutine = RippleCoroutine();
        StartCoroutine(_rippleCoroutine);
    }

    private void OnDrawGizmos()
    {
        foreach (var waterLevel in waterLevels.waterLevels)
        {
            Gizmos.color = waterLevel.color;
            Gizmos.DrawSphere(new Vector3(0, waterLevel.highTideY, 0), 1);
        }
    }

    private void GenerateWaterLevels()
    {
        foreach (var waterLevel in waterLevels.waterLevels)
        {
            var waveMiddle = (waterLevel.upperBorder - waterLevel.lowerBorder) / 2;
            var boxCenter = new Vector3(0, 10, waterLevel.lowerBorder + waveMiddle);
            var halfExtents = new Vector3(transform.localScale.x / 2, 0.01f, waveMiddle);

            if (Physics.BoxCast(boxCenter, halfExtents, Vector3.down, out var raycastHit, Quaternion.identity,
                LayerMask.GetMask(new[] {"Beach"})))
            {
                waterLevel.highTideY = raycastHit.point.y;
            }
        }
    }

    private IEnumerator RippleCoroutine()
    {
        foreach (var waveData in waveValues.waves)
        {
            yield return MoveWaveCoroutine(waterLevels.GetWaterLavel(waveData.lowTideLayer).lowerBorder, waveData.lowTideTime);
            yield return new WaitForSeconds(waveData.highTideTimeout);
            OnWaveUp?.Invoke(this, waveData);
            yield return MoveWaveCoroutine(waterLevels.GetWaterLavel(waveData.highTideLayer).upperBorder, waveData.highTideTime);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator MoveWaveCoroutine(float newWaveHeight, float waveMoveTime)
    {
        var currentPosition = transform.position;
        for(var f = 0f; f <= waveMoveTime; f += Time.deltaTime) {
            var t = f / waveMoveTime;
            t = t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
            transform.position = new Vector3(0, Mathf.Lerp(currentPosition.y, newWaveHeight, t), 0);
            yield return null;
        }
    }
}
