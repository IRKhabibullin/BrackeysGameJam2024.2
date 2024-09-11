using UnityEngine;

public class BeachLayersVisualizer : MonoBehaviour
{
    public WaveLayers_SO waveLayers;

    private static readonly Vector3 layerMarkSize = new(2, 0.001f, 0.5f);

    private void OnDrawGizmos()
    {
        foreach (var waveLayer in waveLayers.waveLayers)
        {
            Gizmos.color = waveLayer.color;
            Gizmos.DrawCube(transform.position + new Vector3(49, 0.01f, waveLayer.upperBorder - 0.2f), layerMarkSize);
            Gizmos.DrawCube(transform.position + new Vector3(49, 0.01f, waveLayer.lowerBorder + 0.2f), layerMarkSize);
        }
    }
}
