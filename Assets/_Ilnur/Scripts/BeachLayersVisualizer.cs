using UnityEngine;

public class BeachLayersVisualizer : MonoBehaviour
{
    public WaterLevels_SO waterLevels;

    private static readonly Vector3 layerMarkSize = new(2, 0.001f, 0.5f);

    private void OnDrawGizmos()
    {
        foreach (var waterLevel in waterLevels.waterLevels)
        {
            Gizmos.color = waterLevel.color;
            Gizmos.DrawCube(transform.position + new Vector3(49, 0.01f, waterLevel.upperBorder - 0.2f), layerMarkSize);
            Gizmos.DrawCube(transform.position + new Vector3(49, 0.01f, waterLevel.lowerBorder + 0.2f), layerMarkSize);
        }
    }
}
