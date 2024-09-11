using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWaveValues", menuName = "Data Objects/Wave values")]
public class WaveValues_SO : ScriptableObject
{
    public List<WaveData> waves;
}

[Serializable]
public class WaveData
{
    [Tooltip("Уровень волны на приливе")] public WaterLevel highTideLayer;
    [Tooltip("Уровень волны на отливе")] public WaterLevel lowTideLayer;
    [Tooltip("Время подъема")] public float highTideTime;
    [Tooltip("Время спуска")] public float lowTideTime;
    [Tooltip("Время до подъема волны")] public float highTideTimeout;
    [Tooltip("Количество полезных предметов")] public int usefulItemsSpawnRate;
    [Tooltip("Количество негативных предметов")] public int uselessItemsSpawnRate;
    [Tooltip("Количество препятствий")] public int obstaclesSpawnRate;
}
