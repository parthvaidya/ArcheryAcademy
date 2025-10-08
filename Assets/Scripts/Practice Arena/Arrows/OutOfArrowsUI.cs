//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;

//public class OutOfArrowsUI : MonoBehaviour
//{
//    [Header("UI References")]
//    [SerializeField] private TextMeshProUGUI scoreText;
//    [SerializeField] private TextMeshProUGUI bestScoreText;
//    [SerializeField] private Button quitButton;

//    private bool isVisible = false;

//    private void Awake()
//    {
//        // Hide popup at start
//        gameObject.SetActive(false);
//    }

//    /// <summary>
//    /// Show the Out Of Arrows popup and update score info.
//    /// </summary>
//    public void Show(int currentScore)
//    {
//        // Update current score
//        if (scoreText != null)
//            scoreText.text = $"Score: {currentScore}";

//        // Update and save best score
//        int best = PlayerPrefs.GetInt("BestScore", 0);
//        if (currentScore > best)
//        {
//            best = currentScore;
//            PlayerPrefs.SetInt("BestScore", best);
//            PlayerPrefs.Save();
//        }

//        if (bestScoreText != null)
//            bestScoreText.text = $"Best: {best}";

//        // Enable popup
//        gameObject.SetActive(true);
//        isVisible = true;
//    }

//    /// <summary>
//    /// Hide the popup cleanly.
//    /// </summary>
//    public void Hide()
//    {
//        if (!isVisible)
//            return;

//        gameObject.SetActive(false);
//        isVisible = false;
//    }
//}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OutOfArrowsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private Button quitButton; // assign in Inspector

    private bool isVisible = false;

    private void Awake()
    {
        // Hide popup at start
        gameObject.SetActive(false);

        // Hook up Quit button event
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitPressed);
    }

    
    public void Show(int currentScore)
    {
        // Update current score
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";

        // Update and save best score
        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (currentScore > best)
        {
            best = currentScore;
            PlayerPrefs.SetInt("BestScore", best);
            PlayerPrefs.Save();
        }

        if (bestScoreText != null)
            bestScoreText.text = $"Best: {best}";

        // Enable popup
        gameObject.SetActive(true);
        isVisible = true;
    }

    /// <summary>
    /// Hide the popup cleanly.
    /// </summary>
    public void Hide()
    {
        if (!isVisible)
            return;

        gameObject.SetActive(false);
        isVisible = false;
    }

    /// <summary>
    /// Handles Quit button — goes back to Main Menu.
    /// </summary>
    private void OnQuitPressed()
    {
        Time.timeScale = 1f; // ensure normal time
        SceneManager.LoadScene("Main Menu");
    }
}
