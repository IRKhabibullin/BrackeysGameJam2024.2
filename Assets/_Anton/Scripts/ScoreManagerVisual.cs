using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManagerVisual : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = ScoreManager.Instance;
        UpdateScore();
        scoreManager.OnScoreChanged += ScoreManager_OnScoreChanged;
    }

    private void ScoreManager_OnScoreChanged(object sender, System.EventArgs e)
    {
        UpdateScore();
    }

    private void UpdateScore()
    {
        scoreText.text = scoreManager.GetCurrentScore().ToString();
    }
}
