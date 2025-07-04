using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; private set; } = 0;
    [SerializeField] private TMP_Text scoreText;

    public void AddScore(int amount)
    {
        Score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {Score}";
    }
} 