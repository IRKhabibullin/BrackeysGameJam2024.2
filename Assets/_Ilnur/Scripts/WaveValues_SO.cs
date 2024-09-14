using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewWaveValues", menuName = "Data Objects/Wave values")]
public class WaveValues_SO : ScriptableObject
{
    public List<WaveData> waves;
}

[Serializable]
public class WaveData
{
    [FormerlySerializedAs("lowTideTimeout")]
    [Header("Low tide")]
    [Tooltip("Время до спуска волны")] public float lowTideDuration;
    [Tooltip("Уровень волны на отливе")] public WaterLevel lowTideLayer;
    [Tooltip("Время спуска")] public float lowTideMoveTime;
    
    [FormerlySerializedAs("highTideTimeout")]
    [Header("High tide")]
    [Tooltip("Время до подъема волны")] public float highTideDuration;
    [Tooltip("Уровень волны на приливе")] public WaterLevel highTideLayer;
    [Tooltip("Время подъема")] public float highTideMoveTime;
    
    [Header("Items")]
    [Tooltip("Количество полезных предметов")] public int usefulItemsSpawnRate;
    [Tooltip("Количество негативных предметов")] public int uselessItemsSpawnRate;
    [Tooltip("Количество препятствий")] public int obstaclesSpawnRate;
    
    [Header("Effects")]
    [Tooltip("Звук волны")] public AudioClip waveSound;
    [Tooltip("Погода, начинающаяся с этой волной")] public WeatherEffect weather;
    // [Tooltip("Музыка, начинающаяся с этой волной")] public MusicEffects Music;
}
