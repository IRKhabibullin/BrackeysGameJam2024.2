using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private static int currentScore;
    [SerializeField] private AudioClip goodItemPickUpSound;
    [SerializeField] private AudioClip badItemPickUpSound;
    [SerializeField] private AudioSource audioSource;
    public static event EventHandler OnScoreChanged;
    public static event EventHandler OnGameReset;
    private static List<int> scoreList = new List<int>() {0};
    private int bestScore = 0;

    private void Start()
    {

    }

    private void ResetScore()
    {
        RecordScore(currentScore);
        currentScore = 0;
        OnGameReset?.Invoke(this, null);
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
        if (e.collectionType == CollectableItemSO.CollectionType.Garbage)
        {
            audioSource.clip = badItemPickUpSound;
            audioSource.Play();
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = goodItemPickUpSound;
                audioSource.Play();
            }
        }
        OnScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    public static int GetCurrentScore()
    {
        return currentScore;
    }

    private void RecordScore(int lastScore)
    {
        scoreList.Add(currentScore);
        scoreList.Sort((a, b) => b.CompareTo(a));
    }

    public static int GetBestScore()
    {
        int bestScore = scoreList[0];
        return bestScore;
    }
}
