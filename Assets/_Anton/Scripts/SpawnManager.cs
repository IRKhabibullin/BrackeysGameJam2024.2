using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SpawnManager : MonoBehaviour
{
    //create sub-zones within zones (divided on horizontal)
    //make a better system for connecting zones, items and probabilities

    [SerializeField] private List<CollectableItem> collectableItemList;
    [SerializeField] private List<CollectableItemSO> collectableItemSOList;
    [SerializeField] private WaveValues_SO waveValues_SO;
    [SerializeField] private WaterLevels_SO waterLevels_SO;

    [SerializeField] private Transform coastPlane;
    [SerializeField] private int maxZoneCount;
    [SerializeField] private int subZoneCount;

    [SerializeField] private Dictionary<int, List<CollectableItemSO>> zonesAndItemsDict = new Dictionary<int, List<CollectableItemSO>>();

    private MeshRenderer planeRenderer;
    private int currentWaveZoneCount;
    private List<SpawnZone> currentWaveZoneList;

    private void OnEnable()
    {
        WaveController.OnWaveUp += WaveController_OnWaveUp;
    }

    private void OnDisable()
    {
        WaveController.OnWaveUp -= WaveController_OnWaveUp;
    }

    private void Start()
    {
        currentWaveZoneList = new List<SpawnZone>();
        planeRenderer = coastPlane.GetComponent<MeshRenderer>();
        FillZonesAndItemsDict(collectableItemSOList);

    }

    //everything what we do when the wave is up
    private void WaveController_OnWaveUp(object sender, WaveData waveData)
    {
        Debug.Log(CalculateCurrentWaveZoneCount((int)waveData.highTideLayer, (int)waveData.lowTideLayer));

        int collectablesSpawnRate = waveData.usefulItemsSpawnRate;
        int currentZoneCount = CalculateCurrentWaveZoneCount((int)waveData.highTideLayer, (int)waveData.lowTideLayer);

        CreateZones(coastPlane, currentZoneCount, waveData);

        for (int m = 0; m < currentZoneCount; m++) //going through zones to spawn items
        {
            for (int i = 0; i < collectablesSpawnRate; i++) // spawning item in each zone
            {
                SpawnItem(currentWaveZoneList[m]);
            }
        }
    }

    public void SpawnItem(SpawnZone zone)
    {
        List<CollectableItem> validObjectSpawnList = new List<CollectableItem> ();
        List<CollectableItemSO> validObjectInThisZoneSO = new List<CollectableItemSO> ();

        zonesAndItemsDict.TryGetValue(zone.zoneId, out validObjectInThisZoneSO);

        //defining which objects can be spawned in a zone
        foreach (CollectableItem collectableItem in collectableItemList)
        {
            CollectableItemSO testCollectableItemSO = collectableItem.GetCollectableItemData();
            for (int i = 0; i < validObjectInThisZoneSO.Count; i++)
            {
                if (testCollectableItemSO == validObjectInThisZoneSO[i])
                {
                    validObjectSpawnList.Add(collectableItem);
                }
            }
        }

        Quaternion randomRotationY = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        Vector3 worldSpawnPosition = (ChooseRandomPositionWithinZone(zone));
        GameObject spawnedObject = Instantiate(ChooseRandomObjectToSpawn(validObjectSpawnList), worldSpawnPosition, randomRotationY);
        Debug.Log("spawnedObject = " + spawnedObject.ToString());
        //spawnedObject.transform.up = coastPlane.transform.up;
    }

    private GameObject ChooseRandomObjectToSpawn(List<CollectableItem> validObjectSpawnList)
    {
        int randomIndex = Random.Range(0, validObjectSpawnList.Count);
        return validObjectSpawnList[randomIndex].gameObject;
    }

    private Vector3 ChooseRandomPositionWithinZone(SpawnZone zone)
    {
        float randomX = Random.Range(zone.leftB, zone.rightB);
        Debug.Log("randomX" + randomX);
        float randomZ = Random.Range(zone.bottomB, zone.topB);
        Debug.Log("randomZ" + randomZ);
        float yCoordinate = GetYCoordinateOnPlane(randomX, randomZ);

        Vector3 position = new Vector3(randomX, yCoordinate, randomZ);
        return position;
    }

    private float GetYCoordinateOnPlane(float xValue, float zValue)
    {
        int highPosition = 10;
        Vector3 rayOrigin = new Vector3(xValue, highPosition, zValue);
        Vector3 rayOriginDirection = Vector3.down;

        Ray yFinderRay = new Ray(rayOrigin, rayOriginDirection);
        float yValue = 99; //stub

        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Beach");
        if (Physics.Raycast(yFinderRay, out hit, Mathf.Infinity, layerMask))
        {
            if(hit.collider.transform == coastPlane)
            {
                yValue = hit.point.y;
            }
        }

        if (yValue == 99)
        {
            Debug.LogError("Y value is not correct: " + yValue);
        }

        return yValue;
    }

    public void CreateZones(Transform spawnPlane, int zoneCount, WaveData waveData)
    {
        currentWaveZoneList.Clear();
        float diffBetweenZones = (FindZoneZCoordinates(waveData)[0] - FindZoneZCoordinates(waveData)[1]) / zoneCount;

        //storing offsets
        float bottomOffset = diffBetweenZones / 4; // offset not to spawn too close to the ocean side, divider is semi-random number. this only works good if zones are of the same size
        float topOffset = diffBetweenZones / 3f; // offset to hide tall items near the end of the wave, divider is semi-random. this only works good if zones are of the same size
        float itemSizeOffset = diffBetweenZones / 10; // making it so that items don't intersect if spawned at boundaries of different zones, the better but harder way would be to calculate size of the biggest object and make an offset for that

        if (zoneCount > 1)
        {
            for (int i = 0; i < zoneCount; i++)
            {
                int highLayerInt = (int)waveData.highTideLayer - i;
                WaterLevel waterLevelTop = (WaterLevel)highLayerInt;

                float topZBoundary = 99; //stub
                float bottomZBoundary = 99; //stub

                List<float> zoneZcoordinates = FindZoneZCoordinates(waterLevelTop);
                if (i == 0) // this is the first zone, closest to the shore
                {
                    topZBoundary = zoneZcoordinates[0] - itemSizeOffset - topOffset;
                    bottomZBoundary = zoneZcoordinates[1] + itemSizeOffset;

                    if (topZBoundary <= bottomZBoundary)
                    {
                        Debug.LogError("Zone boundaries are not correct: " + "top: " + topZBoundary + ", bottom: " + bottomZBoundary);
                    }
                }
                else if (i == zoneCount - 1) //this is the last zone, closest to the sea
                {
                    topZBoundary = zoneZcoordinates[0] - itemSizeOffset;
                    bottomZBoundary = zoneZcoordinates[1] + itemSizeOffset + bottomOffset;

                    if (topZBoundary <= bottomZBoundary)
                    {
                        Debug.LogError("Zone boundaries are not correct: " + "top: " + topZBoundary + ", bottom: " + bottomZBoundary);
                    }
                }
                else //this is any zone inbetween
                {
                    topZBoundary = zoneZcoordinates[0] - itemSizeOffset;
                    bottomZBoundary = zoneZcoordinates[1] + itemSizeOffset;

                    if (topZBoundary <= bottomZBoundary)
                    {
                        Debug.LogError("Zone boundaries are not correct: " + "top: " + topZBoundary + ", bottom: " + bottomZBoundary);
                    }
                }

                float leftBoundary = planeRenderer.bounds.min.x + itemSizeOffset + diffBetweenZones * 2;
                float rightBoundary = planeRenderer.bounds.max.x - itemSizeOffset - diffBetweenZones * 2;

                if (topZBoundary == 99 || bottomZBoundary == 99)
                {
                    Debug.LogError("topZBoundary or bottomZBoundary wasn't properly assigned");
                }
                SpawnZone zone = new SpawnZone(i, waterLevelTop, leftBoundary, rightBoundary, topZBoundary, bottomZBoundary);
                currentWaveZoneList.Add(zone);
                Debug.Log("Zone created: " + zone.ToString());
            }
        }
        else
        {
            Debug.LogError("Zone count is less than 2: " + zoneCount); //it is required to have at least 2 zones to hide objects better beneath the water
        }
    }

    //used to get the number of zones to spawn objects in
    private int CalculateCurrentWaveZoneCount(int highLevel, int lowLevel)
    {
        int zoneCount = Mathf.Abs(highLevel) + Mathf.Abs(lowLevel) + 1; //+1 because of 0 zone
        return zoneCount;
    }

    //this one might be unnecessary, left it just in case, it can calculate Z for the whole spawning area
    private List<float> FindZoneZCoordinates(WaveData waveData)
    {
        float topZcoordinate = 99; //just creating a variable, probably there is a better way
        float bottomZcoordinate = 99; //just creating a variable, probably there is a better way

        foreach (WaterLevelData waterLevelData in waterLevels_SO.GetWaterLevelsList())
        {
            if (waveData.highTideLayer == waterLevelData.level)
            {
                topZcoordinate = waterLevelData.upperBorder;
            }
            if (waveData.lowTideLayer == waterLevelData.level)
            {
                bottomZcoordinate = waterLevelData.lowerBorder;
            }
        }

        if (topZcoordinate == 99 || bottomZcoordinate == 99)
        {
            Debug.LogError("topZcoordinate or bottomZcoordinate wasn't properly assigned");
        }

        List<float> verticalCoordinates = new List<float>() { topZcoordinate, bottomZcoordinate };
        return verticalCoordinates;
    }

    //returning Z of a specific coast zone
    private List<float> FindZoneZCoordinates(WaterLevel topLevel)
    {
        float topZcoordinate = 99; //just creating a variable, probably there is a better way
        float bottomZcoordinate = 99; //just creating a variable, probably there is a better way

        foreach (WaterLevelData waterLevelData in waterLevels_SO.GetWaterLevelsList())
        {
            if (topLevel == waterLevelData.level)
            {
                topZcoordinate = waterLevelData.upperBorder;
                bottomZcoordinate = waterLevelData.lowerBorder;
            }
        }

        if (topZcoordinate == 99 || bottomZcoordinate == 99)
        {
            Debug.LogError("topZcoordinate or bottomZcoordinate wasn't properly assigned");
        }

        List<float> verticalCoordinates = new List<float>() { topZcoordinate, bottomZcoordinate };
        return verticalCoordinates;
    }

    private void FillZonesAndItemsDict(List<CollectableItemSO> listOfItemsSO)
    {
        //this is done in an ugly-hardcody style

        zonesAndItemsDict.Add(0, new List<CollectableItemSO> { listOfItemsSO[0], listOfItemsSO[1] });
        zonesAndItemsDict.Add(1, new List<CollectableItemSO> { listOfItemsSO[0], listOfItemsSO[1] });
        zonesAndItemsDict.Add(2, new List<CollectableItemSO> { listOfItemsSO[1], listOfItemsSO[2] });
        zonesAndItemsDict.Add(3, new List<CollectableItemSO> { listOfItemsSO[2], listOfItemsSO[3] });
        zonesAndItemsDict.Add(4, new List<CollectableItemSO> { listOfItemsSO[3], listOfItemsSO[4] });
        zonesAndItemsDict.Add(5, new List<CollectableItemSO> { listOfItemsSO[4], listOfItemsSO[5] });
        zonesAndItemsDict.Add(6, new List<CollectableItemSO> { listOfItemsSO[4], listOfItemsSO[5], listOfItemsSO[6] });
        zonesAndItemsDict.Add(7, new List<CollectableItemSO> { listOfItemsSO[5], listOfItemsSO[6] });
    }
}
