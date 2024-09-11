using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CollectableItemSO : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
    public int pointsValue;
    public int shopCost;
    public CollectionType collectionType;
    public enum CollectionType
    {
        Shells,
        Coins,
        Gems,
        Garbage
    }

}
