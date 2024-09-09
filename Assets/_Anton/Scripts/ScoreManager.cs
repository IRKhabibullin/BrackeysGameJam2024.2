using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private int currentScore;
    [SerializeField] public event EventHandler OnScoreChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than on LevelGrid! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        CollectableItem.OnAnyCollectableItemPicked += CollectableItem_OnAnyCollectableItemPicked;
    }

    private void OnDisable()
    {
        CollectableItem.OnAnyCollectableItemPicked -= CollectableItem_OnAnyCollectableItemPicked;
    }

    private void CollectableItem_OnAnyCollectableItemPicked(object sender, CollectableItem.OnPickedEventArgs e)
    {
        currentScore += e.collectableItemSO.pointsValue;
        OnScoreChanged?.Invoke(this, e);

    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}
