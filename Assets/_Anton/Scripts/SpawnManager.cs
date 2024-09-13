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
    [SerializeField] private List<CollectableItem> garbageList;
    [SerializeField] private WaveValues_SO waveValues_SO;
    [SerializeField] private WaterLevels_SO waterLevels_SO;

    [SerializeField] private Transform coastPlane;
    [SerializeField] private int maxZoneCount;
    [SerializeField] private int maxGarbageCount; // maximum of garbage on the ground
    [SerializeField] private int startingGarbageZone; //in zones with lower index garbage will not spawn

    [SerializeField] private Dictionary<int, List<CollectableItemSO>> zonesAndItemsDict = new Dictionary<int, List<CollectableItemSO>>();

    private MeshRenderer planeRenderer;
    private int currentWaveZoneCount;
    private List<SpawnZone> currentWaveZoneList;
    private List<GameObject> objectsForRemoval;
    private int garbageCount = 0;

    private enum TypeOfObject
    {
        Useful,
        Useless,
        Obstacle,
    }

    private void OnEnable()
    {
        WaveController.OnWaveUp += WaveController_OnWaveUp;
        ScoreManager.OnGameReset += ScoreManager_OnGameReset;
    }

    private void OnDisable()
    {
        WaveController.OnWaveUp -= WaveController_OnWaveUp;
        ScoreManager.OnGameReset -= ScoreManager_OnGameReset;
    }

    private void Start()
    {
        currentWaveZoneList = new List<SpawnZone>();
        objectsForRemoval = new List<GameObject>();
        planeRenderer = coastPlane.GetComponent<MeshRenderer>();
        FillZonesAndItemsDict(collectableItemSOList);
    }

    //everything what we do when the wave is up
    private void WaveController_OnWaveUp(object sender, WaveData waveData)
    {
        int collectablesSpawnRate = waveData.usefulItemsSpawnRate;
        int garbageSpawnRate = waveData.uselessItemsSpawnRate;
        int currentZoneCount = CalculateCurrentWaveZoneCount((int)waveData.highTideLayer, (int)waveData.lowTideLayer);

        CreateZones(coastPlane, currentZoneCount, waveData);

        //spawning usefull items
        for (int m = 0; m < currentZoneCount; m++) //going through zones to spawn items
        {
            for (int i = 0; i < collectablesSpawnRate; i++) // spawning item in each zone
            {
                SpawnItem(currentWaveZoneList[m], TypeOfObject.Useful);
            }
        }

        //spawning useless items if the limit was not achieved
        if (garbageCount < maxGarbageCount)
        {
            if (currentZoneCount >= startingGarbageZone)
            {
                int zoneIndexForGarbage = Random.Range(startingGarbageZone - 1, currentZoneCount); //minus 1 because list of zones starts from 0

                for (int i = 0; i < garbageSpawnRate; i++)
                {
                    if (garbageCount < maxGarbageCount)
                    {
                        SpawnItem(currentWaveZoneList[zoneIndexForGarbage], TypeOfObject.Useless);
                        garbageCount++;
                    }
                }
            }
        }
    }

    private void SpawnItem(SpawnZone zone, TypeOfObject typeOfObject)
    {
        List<CollectableItem> validObjectSpawnList = new List<CollectableItem>();

        switch (typeOfObject)
        {
            case TypeOfObject.Useful:
                validObjectSpawnList = GetUsefulObjectListForZone(zone);
                break;

            case TypeOfObject.Useless:
                validObjectSpawnList = garbageList;
                break;

            case TypeOfObject.Obstacle:
                validObjectSpawnList = null;
                break;
        }
        Quaternion randomRotationY = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        Vector3 worldSpawnPosition = (ChooseRandomPositionWithinZone(zone));
        GameObject spawnedObject = Instantiate(ChooseRandomObjectToSpawn(validObjectSpawnList), worldSpawnPosition, randomRotationY);
        objectsForRemoval.Add(spawnedObject);
        //spawnedObject.transform.up = coastPlane.transform.up;
    }

    private List<CollectableItem> GetUsefulObjectListForZone(SpawnZone zone)
    {
        List<CollectableItem> validObjectSpawnList = new List<CollectableItem>();
        List<CollectableItemSO> validObjectInThisZoneSO = new List<CollectableItemSO>();

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
        return validObjectSpawnList;
    }

    private GameObject ChooseRandomObjectToSpawn(List<CollectableItem> validObjectSpawnList)
    {
        int randomIndex = Random.Range(0, validObjectSpawnList.Count);
        return validObjectSpawnList[randomIndex].gameObject;
    }

    private Vector3 ChooseRandomPositionWithinZone(SpawnZone zone)
    {
        float randomX = Random.Range(zone.leftB, zone.rightB);
        float randomZ = Random.Range(zone.bottomB, zone.topB);
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
            if (hit.collider.transform == coastPlane)
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
        zonesAndItemsDict.Add(7, new List<CollectableItemSO> { listOfItemsSO[6] });
    }

    private void ScoreManager_OnGameReset(object sender, System.EventArgs e)
    {
        garbageCount = 0;
        ClearSceneFromObjects();
    }

    private void ClearSceneFromObjects()
    {
        foreach (GameObject gameObject in objectsForRemoval)
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
