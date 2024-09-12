using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private static int currentScore;
    public static event EventHandler OnScoreChanged;

    private void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(this, null);
    }

    private void OnEnable()
    {
        CollectableItem.OnAnyCollectableItemPicked += CollectableItem_OnAnyCollectableItemPicked;
        MovementController.OnWaterTouched += ResetScore;
    }

    private void OnDisable()
    {
        CollectableItem.OnAnyCollectableItemPicked -= CollectableItem_OnAnyCollectableItemPicked;
        MovementController.OnWaterTouched -= ResetScore;
    }

    private void CollectableItem_OnAnyCollectableItemPicked(object sender, CollectableItemSO e)
    {
        currentScore += e.pointsValue;
        OnScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    public static int GetCurrentScore()
    {
        return currentScore;
    }
}
