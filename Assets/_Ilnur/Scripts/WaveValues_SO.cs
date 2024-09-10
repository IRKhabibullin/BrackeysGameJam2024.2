using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWaveValues", menuName = "Data Objects/Wave values")]
public class WaveValues_SO : ScriptableObject
{
    public List<WaveData> waves;
}

[CreateAssetMenu(fileName = "NewWaveLayers", menuName = "Data Objects/Wave layers")]
public class WaveLayers_SO : ScriptableObject
{
    public List<WaveLayerData> waveLayers;

    public WaveLayerData GetWaveLayer(WaveLayer layerId)
    {
        return waveLayers.First(x => x.layer == layerId);
    }
}

[Serializable]
public class WaveData
{
    [Tooltip("Уровень волны на приливе")] public WaveLayer highTideLayer;
    [Tooltip("Уровень волны на отливе")] public WaveLayer lowTideLayer;
    [Tooltip("Время подъема")] public float highTideTime;
    [Tooltip("Время спуска")] public float lowTideTime;
    [Tooltip("Время до подъема волны")] public float highTideTimeout;
    [Tooltip("Количество полезных предметов")] public int usefulItemsSpawnRate;
    [Tooltip("Количество негативных предметов")] public int uselessItemsSpawnRate;
    [Tooltip("Количество препятствий")] public int obstaclesSpawnRate;

    public float highTideLevel;
    public float lowTideLevel;
}

public enum WaveLayer
{
    Up3 = 3,
    Up2 = 2,
    Up1 = 1,
    SeaLevel = 0,
    Down1 = -1,
    Down2 = -2,
    Down3 = -3,
    DeepWater = -4
}

[Serializable]
public class WaveLayerData
{
    public WaveLayer layer;
    public Color color;
    public float upperBorder;
    public float lowerBorder;
}
