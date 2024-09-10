using System;
using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private WaveValues_SO waveValues;
    [SerializeField] private WaveLayers_SO waveLayers;
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

    private void GenerateWaterLevels()
    {
        foreach (var waveData in waveValues.waves)
        {
            var waveLayer = waveLayers.GetWaveLayer(waveData.highTideLayer);
            var highWaterPlane = new Plane(transform.up, new Vector3(0, waveLayer.upperBorder, 0));
            waveData.highTideLevel = GetWaterLevel(highWaterPlane);
            var lowWaterPlane = new Plane(transform.up, new Vector3(0, waveLayer.lowerBorder, 0));
            waveData.lowTideLevel = GetWaterLevel(lowWaterPlane);
        }
    }

    private float GetWaterLevel(Plane waterPlane)
    {
        var linePoint = Vector3.zero;
        var planeNormal = waterPlane.normal;

        var lineDirection = Vector3.Cross(beachPlane.up, Vector3.Cross(planeNormal, beachPlane.up));

        var numerator = Vector3.Dot(planeNormal, lineDirection);

        if (Mathf.Abs(numerator) > 0.000001f)
        {
            var planeToPlane = waterPlane.ClosestPointOnPlane(transform.position) - beachPlane.position;
            var t = Vector3.Dot(planeNormal, planeToPlane) / numerator;
            linePoint = beachPlane.position + t * lineDirection;
        }

        return linePoint.z;
    }

    private IEnumerator RippleCoroutine()
    {
        foreach (var waveData in waveValues.waves)
        {
            yield return MoveWaveCoroutine(waveLayers.GetWaveLayer(waveData.lowTideLayer).lowerBorder, waveData.lowTideTime);
            yield return new WaitForSeconds(waveData.highTideTimeout);
            OnWaveUp?.Invoke(this, waveData);
            yield return MoveWaveCoroutine(waveLayers.GetWaveLayer(waveData.highTideLayer).upperBorder, waveData.highTideTime);
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
