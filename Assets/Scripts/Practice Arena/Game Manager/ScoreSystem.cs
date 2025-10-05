using TMPro;

public class ScoreSystem
{
    private TextMeshProUGUI scoreText;
    public int CurrentScore { get; private set; }

    public delegate void ScoreChanged(int newScore);
    public event ScoreChanged OnScoreChanged;

    public ScoreSystem(TextMeshProUGUI uiText)
    {
        scoreText = uiText;
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        UpdateUI();
        OnScoreChanged?.Invoke(CurrentScore);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {CurrentScore}";
    }
}