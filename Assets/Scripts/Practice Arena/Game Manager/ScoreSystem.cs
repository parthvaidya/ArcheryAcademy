
using TMPro;
using UnityEngine;

public class ScoreSystem
{
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI bestScoreText;

    public int CurrentScore { get; private set; }
    public int BestScore { get; private set; }

    private const string BEST_SCORE_KEY = "BestScore";

    public delegate void ScoreChanged(int newScore);
    public event ScoreChanged OnScoreChanged;

    public ScoreSystem(TextMeshProUGUI uiText, TextMeshProUGUI bestText)
    {
        scoreText = uiText;
        bestScoreText = bestText;

        LoadBestScore();
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        UpdateUI();

        //  Check and update best score
        if (CurrentScore > BestScore)
        {
            BestScore = CurrentScore;
            SaveBestScore();
            UpdateUI();
        }

        OnScoreChanged?.Invoke(CurrentScore);
    }

    private void LoadBestScore()
    {
        BestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
    }

    private void SaveBestScore()
    {
        PlayerPrefs.SetInt(BEST_SCORE_KEY, BestScore);
        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {CurrentScore}";

        if (bestScoreText != null)
            bestScoreText.text = $"Best: {BestScore}";
    }

    public void SetScore(int value)
    {
        CurrentScore = value;
        UpdateUI();
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        UpdateUI();
    }
}
