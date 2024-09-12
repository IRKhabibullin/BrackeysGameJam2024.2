using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] CollectableItemSO collectableItemSO;
    public static event EventHandler <CollectableItemSO> OnAnyCollectableItemPicked;
    private String playerLayerMaskString = "Player"; //hardcoded for speed


    //use to get item data
    public CollectableItemSO GetCollectableItemData()
    {
        return collectableItemSO;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerMaskString))
        {
            PickItem();
        }
    }

    //use when you need to pickup item
    public void PickItem()
    {
        OnAnyCollectableItemPicked?.Invoke(this, collectableItemSO);
        DestroySelf();
    }

    //use to destroy
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
