

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GoogleMobileAds.Api;

public class OutOfArrowsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private Button quitButton;

    [SerializeField] private Button add50Button;
    [SerializeField] private Button add100Button;
    [SerializeField] private Button add200Button;

    private ShootArrow shootArrow;
    private RewardedAd rewardedAd;
    private int pendingReward = 0;
    private bool isVisible = false;
    

    private void Awake()
    {
        gameObject.SetActive(false);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitPressed);

        if (add50Button != null)
            add50Button.onClick.AddListener(() => OnWatchAdPressed(50));
        if (add100Button != null)
            add100Button.onClick.AddListener(() => OnWatchAdPressed(100));
        if (add200Button != null)
            add200Button.onClick.AddListener(() => OnWatchAdPressed(200));

        shootArrow = FindFirstObjectByType<ShootArrow>();
        if (shootArrow == null)
            Debug.LogWarning("No ShootArrow found in the scene!");
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });
       
    }

    private void SetButtonsInteractable(bool value)
    {
        add50Button.interactable = value;
        add100Button.interactable = value;
        add200Button.interactable = value;
    }

   
    public void Initialize(ShootArrow shooter)
    {
        shootArrow = shooter;
        Debug.Log(" ShootArrow reference injected successfully!");
    }

    //private void OnWatchAdPressed(int arrowsToAdd)
    //{
    //    if (RewardedAdsManager.Instance != null)
    //    {
    //        RewardedAdsManager.Instance.ShowAd(() =>
    //        {
    //            // reward callback
    //            if (shootArrow != null)
    //                shootArrow.AddArrows(arrowsToAdd);

    //            Hide();
    //        });
    //    }
    //    else
    //    {
    //        Debug.LogWarning("RewardedAdsManager not found!");
    //    }
    //}

    private void OnWatchAdPressed(int arrowsToAdd)
    {
        pendingReward = arrowsToAdd;

        if (RewardedAdsManager.Instance != null)
        {
            if (RewardedAdsManager.Instance.IsAdReady)   // <-- check if preloaded
            {
                RewardedAdsManager.Instance.ShowAd(() =>
                {
                    shootArrow?.AddArrows(pendingReward);
                    pendingReward = 0;
                    Hide();
                });
            }
            else
            {
                Debug.LogWarning("Ad not ready yet. Reloading...");
                RewardedAdsManager.Instance.LoadAd(); // try again
            }
        }
    }



    public void Show(int currentScore)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";

        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (currentScore > best)
        {
            best = currentScore;
            PlayerPrefs.SetInt("BestScore", best);
            PlayerPrefs.Save();
        }

        if (bestScoreText != null)
            bestScoreText.text = $"Best: {best}";

        gameObject.SetActive(true);
        isVisible = true;

        // Only enable reward buttons if ad is ready
        SetButtonsInteractable(RewardedAdsManager.Instance != null && RewardedAdsManager.Instance.IsAdReady);
    }

    public void Hide()
    {
        if (!isVisible) return;
        gameObject.SetActive(false);
        isVisible = false;
    }

    private void OnQuitPressed()
    {
        Time.timeScale = 1f;
        if (SessionManager.Instance != null && SessionManager.Instance.ShouldShowAds)
        {
            if (InterstitialAdsManager.Instance != null)
                InterstitialAdsManager.Instance.ShowAd();
        }
        SceneManager.LoadScene("Main Menu");
    }
}



