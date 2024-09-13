using System;

public struct SpawnZone
{
    //B = boundary
    public int zoneId;
    public WaterLevel waterLevel;
    public float leftB;
    public float rightB;
    public float topB;
    public float bottomB;

    public SpawnZone(int zoneId, WaterLevel waterLevel, float leftB, float rightB, float topB, float bottomB)
    {
        this.zoneId = zoneId;
        this.waterLevel = waterLevel;
        this.leftB = leftB;
        this.rightB = rightB;
        this.topB = topB;
        this.bottomB = bottomB;
    }
}

