using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CollectableItemSO : ScriptableObject
{
    //public Transform prefab;
    //public Sprite sprite;
    //public string objectName;
    public int pointsValue;
    //public int shopCost;
    public CollectionType collectionType;
    public enum CollectionType
    {
        Sticks = 1,
        Coins = 2,
        Seashells = 3,
        Starfish = 4,
        Crystalls = 5,
        Pebbles = 6,
        Treasures = 7,
    }

}
