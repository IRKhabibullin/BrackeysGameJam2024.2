using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManagerVisual : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateScore();
        ScoreManager.OnScoreChanged += ScoreManager_OnScoreChanged;
    }

    private void ScoreManager_OnScoreChanged(object sender, System.EventArgs e)
    {
        UpdateScore();
    }

    private void UpdateScore()
    {
        scoreText.text = ScoreManager.GetCurrentScore().ToString();
    }
}
