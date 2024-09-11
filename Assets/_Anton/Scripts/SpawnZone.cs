using System;

public struct SpawnZone
{
    //B = boundary
    public int zoneId;
    public float leftB;
    public float rightB;
    public float topB;
    public float bottomB;

    public SpawnZone(int zoneId, float leftB, float rightB, float topB, float bottomB)
    {
        this.zoneId = zoneId;
        this.leftB = leftB;
        this.rightB = rightB;
        this.topB = topB;
        this.bottomB = bottomB;

    }
}

