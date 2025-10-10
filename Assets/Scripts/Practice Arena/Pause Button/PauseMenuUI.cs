using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Button References")]
    public Button continueButton;
    public Button restartButton;
    public Button quitButton;
    public Button closeButton;

    [Header("Assign in Inspector")]
    public PauseButton pauseButtonScript; // direct link to PauseButton

    private void Awake()
    {
        // Hook up buttons
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinuePressed);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnContinuePressed);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartPressed);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitPressed);
    }

    private void OnContinuePressed()
    {
        if (pauseButtonScript != null)
            pauseButtonScript.ResumeGame();
    }

    private void OnRestartPressed()
    {
        // Save current score before reloading
        if (SessionManager.Instance != null && SessionManager.Instance.ShouldShowAds && SessionManager.Instance.CanShowInterstitial())
        {
            if (InterstitialAdsManager.Instance != null)
                InterstitialAdsManager.Instance.ShowAd();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnQuitPressed()
    {
        Time.timeScale = 1f;

        

        //if (SessionManager.Instance != null && SessionManager.Instance.ShouldShowAds)
        //{
        //    // Show interstitial
        //    if (InterstitialAdsManager.Instance != null)
        //        InterstitialAdsManager.Instance.ShowAd();
        //}
        SceneManager.LoadScene("Main Menu");
    }

    public static class PersistentData
    {
        public static int LastScore = 0;
    }
}
