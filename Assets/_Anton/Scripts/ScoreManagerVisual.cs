using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManagerVisual : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI bestScoreText;

    private void Start()
    {
        UpdateScore();
        UpdateBestScore();
    }

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += ScoreManager_OnScoreChanged;
        ScoreManager.OnGameReset += ScoreManager_OnGameReset;
    }

    private void OnDisable()
    {
        ScoreManager.OnGameReset -= ScoreManager_OnGameReset;
        ScoreManager.OnGameReset -= ScoreManager_OnGameReset;
    }

    private void ScoreManager_OnScoreChanged(object sender, System.EventArgs e)
    {
        UpdateScore();
    }

    private void ScoreManager_OnGameReset(object sender, System.EventArgs e)
    {
        UpdateBestScore();
    }

    private void UpdateScore()
    {
        scoreText.text = ScoreManager.GetCurrentScore().ToString();
    }

    private void UpdateBestScore()
    {
        bestScoreText.text = ScoreManager.GetBestScore().ToString();
    }
}
