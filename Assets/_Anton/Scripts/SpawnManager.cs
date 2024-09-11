using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //TO-DO:
    //subscribe to the event when of a wave hitting its max distance
    //create sub-zones within zones (divided on horizontal)
    //remove Debug.Logs and testing methods/variables
    //make a better system for connecting zones, items and probabilities

    [SerializeField] private List<CollectableItem> collectableItemList;

    [SerializeField] private Transform spawnPlane;
    [SerializeField] private int maxZoneCount;
    [SerializeField] private int maxSubZoneCount;

    private List<SpawnZone> zoneList;
    private int defaultPlaneHalfSize = 5;

    private float testingTimer = 0f;

    private void Awake()
    {

    }

    private void Start()
    {
        zoneList = new List<SpawnZone>();
        CreateZones(spawnPlane, maxZoneCount);
    }

    private void Update()
    {
        testingTimer += Time.deltaTime;
        if (testingTimer > 3f)
        {
            TestingSpawn();
            testingTimer = 0f;
        }
    }

    public void TestingSpawn()
    {
        SpawnItem(zoneList[0]);
        SpawnItem(zoneList[1]);
        SpawnItem(zoneList[2]);
    }

    public void SpawnItem(SpawnZone zone)
    {
        //defining which objects can be spawned in certain zones. this should be remade for something better
        List<int> validObjectSpawnList = new List<int>();
        switch (zone.zoneId)
        {
            case 0:
                validObjectSpawnList = new List<int> { 0 };
                break;
            case 1:
                validObjectSpawnList = new List<int> { 1 };
                break;
            case 2:
                validObjectSpawnList = new List<int> { 2 };
                break;
        }
        //making it so the object is adjusted to the plane position
        Vector3 worldSpawnPosition = spawnPlane.TransformPoint(ChooseRandomPositionWithinZone(zone));
        GameObject spawnedObject = Instantiate(ChooseRandomObjectToSpawn(validObjectSpawnList), worldSpawnPosition, Quaternion.identity);
        spawnedObject.transform.up = spawnPlane.transform.up;
    }

    private GameObject ChooseRandomObjectToSpawn(List<int> validObjectSpawnList)
    {
        //chosing an object which is eligable for the zone
        int randomIndex = Random.Range(0, validObjectSpawnList.Count);
        int randomValidIndex = validObjectSpawnList[randomIndex];
        return collectableItemList[randomValidIndex].gameObject;
    }

    private Vector3 ChooseRandomPositionWithinZone(SpawnZone zone)
    {
        float randomIndexX = Random.Range(zone.leftB, zone.rightB);
        float randomIndexZ = Random.Range(zone.bottomB, zone.topB);
        Vector3 position = new Vector3(randomIndexX, 0, randomIndexZ);
        return position;
    }

    //Is it needed to remake this to base upon bottom and top of wave instead of bottom and top of spawn plane?
    public void CreateZones(Transform spawnPlane, int zoneCount)
    {
        //calculating size of plane which needs to be devided
        //Vector3 localScale = spawnPlane.localScale;
        float initialZoneWidth = defaultPlaneHalfSize; // half localScale.x * 
        float initialZoneHight = defaultPlaneHalfSize; // half localScale.z * 
        float diffBetweenZones = initialZoneHight * 2 / zoneCount; //*localScale.z
        //storing offsets
        float bottomOffset = diffBetweenZones / 7; // offset not to spawn too close to the ocean side
        float topOffset = diffBetweenZones / 5; // offset to hide tall items near the end of the wave
        float itemSizeOffset = diffBetweenZones / 10; // stub value for now, making it so that items don't intersect if spawned at boundaries of different zones

        //Testing
        //Debug.Log("initialZoneHight = " + initialZoneHight);
        //Debug.Log("initialZoneWidth = " + initialZoneWidth);
        //Debug.Log("diffBetweenZones = " + diffBetweenZones);

        for (int step = 0; step < maxZoneCount; step++)
        {
            //creating stub top and bottom to avoid error in SpawnZone creation
            float topB = 0;
            float bottomB = 0;

            //cycling to set zone boundaries
            if (step == 0)
            {
                topB = -initialZoneHight + (step + 1) * diffBetweenZones - itemSizeOffset;
                bottomB = -initialZoneHight + bottomOffset;
            }
            else if (step == maxZoneCount - 1)
            {
                topB = initialZoneHight - topOffset;
                bottomB = initialZoneHight - diffBetweenZones + itemSizeOffset;
            }
            else //for zones inbetween first and last
            {
                topB = -initialZoneHight + (step + 1) * diffBetweenZones - itemSizeOffset;
                bottomB = -initialZoneHight + step * diffBetweenZones + itemSizeOffset;
            }
            float leftB = -initialZoneWidth + itemSizeOffset;
            float rightB = initialZoneWidth - itemSizeOffset;
            int zoneId = step;

            SpawnZone zone = new SpawnZone(zoneId, leftB, rightB, topB, bottomB);
            zoneList.Add(zone);

            //Testing
            Debug.Log("zoneId: " + zoneId);
            //Debug.Log("leftB: " + leftB);
            //Debug.Log("rightB: " + rightB);
            Debug.Log("topB: " + topB);
            Debug.Log("bottomB: " + bottomB);
            Debug.Log("============");
        }
    }


    //Calculate boundaries of a big plane - DONE
    //Divide it by sections each with its own boundaries - DONE
    //Spawn only VALID objects (valid types) in each section


}
