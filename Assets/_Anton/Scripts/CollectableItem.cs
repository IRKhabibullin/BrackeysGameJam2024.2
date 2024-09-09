using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] CollectableItemSO collectableItemSO;
    public static event EventHandler <OnPickedEventArgs> OnAnyCollectableItemPicked;
    private String playerLayerMaskString = "Player"; //hardcoded for speed

    public class OnPickedEventArgs : EventArgs
    {
        public CollectableItemSO collectableItemSO;
    }

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
        OnAnyCollectableItemPicked?.Invoke(this, new OnPickedEventArgs
        {
            collectableItemSO = GetCollectableItemData(),
        });
        DestroySelf();
    }

    //use to destroy
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
