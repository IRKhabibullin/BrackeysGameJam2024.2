using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveController : MonoBehaviour
{
    [Range(-2, 0)] [SerializeField] private float lowWaveHeight;
    [Range(0, 2)] [SerializeField] private float highWaveHeight;

    [SerializeField] private float ripplePeriod;
    [SerializeField] private float rippleSpeed;
    private IEnumerator _rippleCoroutine;
    private WaitForSeconds _rippleWaitForSeconds;
    private bool _lastDirection;

    void Start()
    {
        _rippleCoroutine = RippleCoroutine();
        StartCoroutine(_rippleCoroutine);
        _rippleWaitForSeconds = new WaitForSeconds(ripplePeriod);
    }

    private IEnumerator RippleCoroutine()
    {
        while (true)
        {
            var currentPosition = transform.position;
            var newWaveHeight = _lastDirection ? Random.Range(0, highWaveHeight) : Random.Range(lowWaveHeight, 0);
            var t = 0f;
            Debug.Log(newWaveHeight);
            while (!Mathf.Approximately(currentPosition.y, newWaveHeight))
            {
                currentPosition = new Vector3(0, Mathf.Lerp(currentPosition.y, newWaveHeight, t), 0);
                transform.position = currentPosition;
                
                t += rippleSpeed * Time.deltaTime;
                yield return null;
            }

            _lastDirection = !_lastDirection;
            
            yield return _rippleWaitForSeconds;
        }
    }
}
