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
    [Tooltip("Время до подъема")] public float lowerHeightTime;
    [Tooltip("Время до спуска")] public float upperHeightTime;
    [Tooltip("Количество полезных предметов")] public int usefulItemsSpawnRate;
    [Tooltip("Количество негативных предметов")] public int uselessItemsSpawnRate;
    [Tooltip("Количество препятствий")] public int obstaclesSpawnRate;
}
