using UnityEngine;
using TMPro;
using static PauseMenuUI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText; 
    [SerializeField] private TextMeshProUGUI comboText;

    [Header("Combo Settings")]
    [SerializeField] private float comboResetTime = 2f;
    [SerializeField] private float textDisplayTime = 3f;
    [SerializeField] private float floatUpDistance = 50f;
    [SerializeField] private float fadeOutDuration = 0.6f;

    private ScoreSystem scoreSystem;
    private ComboSystem comboSystem;

    private void Awake()
    {
        Instance = this;

        scoreSystem = new ScoreSystem(scoreText, bestScoreText);
        comboSystem = new ComboSystem(comboText, comboResetTime, textDisplayTime, floatUpDistance, fadeOutDuration);

        // Only restore if coming from another scene, not restart
        if (SceneManager.GetActiveScene().name != "Main Menu" && PersistentData.LastScore > 0)
        {
            scoreSystem.SetScore(PersistentData.LastScore);
            PersistentData.LastScore = 0; 
        }
        else
        {
            scoreSystem.ResetScore(); 
        }

        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        comboSystem.Update();
    }

    public void AddScore(int amount)
    {
        scoreSystem.AddScore(amount);
    }

    public void RegisterHit()
    {
        comboSystem.RegisterHit();
    }

    public void RegisterMiss()
    {
        comboSystem.RegisterMiss();
    }

    public int GetScore() => scoreSystem.CurrentScore;
}
