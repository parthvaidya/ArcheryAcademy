using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            scoreSystem = new ScoreSystem(scoreText);
            comboSystem = new ComboSystem(comboText, comboResetTime, textDisplayTime, floatUpDistance, fadeOutDuration);

            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(gameObject);
        }
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

   