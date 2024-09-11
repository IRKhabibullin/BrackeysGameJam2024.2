using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "NewWaterLevels", menuName = "Data Objects/Water Levels")]
public class WaterLevels_SO : ScriptableObject
{
    public List<WaterLevelData> waterLevels;

    public WaterLevelData GetWaterLavel(WaterLevel level)
    {
        return waterLevels.First(x => x.level == level);
    }
}

public enum WaterLevel
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
public class WaterLevelData
{
    public WaterLevel level;
    public Color color;
    public float upperBorder;
    public float lowerBorder;
    
    [NonSerialized] public float highTideY;
}