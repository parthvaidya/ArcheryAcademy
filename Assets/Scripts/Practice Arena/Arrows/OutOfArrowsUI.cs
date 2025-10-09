

////using UnityEngine;
////using UnityEngine.UI;
////using UnityEngine.SceneManagement;
////using TMPro;
////using GoogleMobileAds.Api;

////public class OutOfArrowsUI : MonoBehaviour
////{
////    [Header("UI References")]
////    [SerializeField] private TextMeshProUGUI scoreText;
////    [SerializeField] private TextMeshProUGUI bestScoreText;
////    [SerializeField] private Button quitButton;

////    [SerializeField] private Button add50Button;
////    [SerializeField] private Button add100Button;
////    [SerializeField] private Button add200Button;

////    private ShootArrow shootArrow;
////    private RewardedAd rewardedAd;
////    private int pendingReward = 0;
////    private bool isVisible = false;
////    private bool isAdReady = false;

////    private void Awake()
////    {
////        gameObject.SetActive(false);

////        if (quitButton != null)
////            quitButton.onClick.AddListener(OnQuitPressed);

////        if (add50Button != null)
////            add50Button.onClick.AddListener(() => OnWatchAdPressed(50));
////        if (add100Button != null)
////            add100Button.onClick.AddListener(() => OnWatchAdPressed(100));
////        if (add200Button != null)
////            add200Button.onClick.AddListener(() => OnWatchAdPressed(200));

////        shootArrow = FindFirstObjectByType<ShootArrow>();
////        if (shootArrow == null)
////            Debug.LogWarning("No ShootArrow found in the scene!");
////    }

////    private void Start()
////    {
////        MobileAds.Initialize(initStatus => { });
////        LoadRewardedAd();
////    }

////    private void SetButtonsInteractable(bool value)
////    {
////        add50Button.interactable = value;
////        add100Button.interactable = value;
////        add200Button.interactable = value;
////    }

////    private void LoadRewardedAd()
////    {
////#if UNITY_ANDROID
////        string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // TEST ad
////#elif UNITY_IOS
////        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
////#else
////        string adUnitId = "unused";
////#endif

////        Debug.Log("Loading Rewarded Ad...");
////        isAdReady = false;
////        SetButtonsInteractable(false); //  disable until loaded

////        AdRequest adRequest = new AdRequest();

////        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
////        {
////            if (error != null)
////            {
////                Debug.LogError("Rewarded ad failed to load: " + error);
////                rewardedAd = null;
////                isAdReady = false;
////                SetButtonsInteractable(false);
////                // Retry after a delay
////                Invoke(nameof(LoadRewardedAd), 3f);
////                return;
////            }

////            rewardedAd = ad;
////            isAdReady = true;
////            SetButtonsInteractable(true); // enable buttons once ad is ready
////            Debug.Log(" Rewarded ad loaded and ready!");

////            rewardedAd.OnAdFullScreenContentClosed += () =>
////            {
////                Debug.Log("Rewarded ad closed. Reloading...");
////                if (shootArrow != null)
////                    shootArrow.LockInputForSeconds(0.5f);
////                LoadRewardedAd();
////            };

////            rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
////            {
////                Debug.LogError("Ad failed to show: " + adError);
////                LoadRewardedAd();
////            };
////        });
////    }

////    public void Initialize(ShootArrow shooter)
////    {
////        shootArrow = shooter;
////        Debug.Log(" ShootArrow reference injected successfully!");
////    }

////    private void OnWatchAdPressed(int arrowsToAdd)
////    {
////        pendingReward = arrowsToAdd;

////        if (rewardedAd != null && isAdReady)
////        {
////            Debug.Log("Showing rewarded ad...");
////            isAdReady = false;
////            SetButtonsInteractable(false);

////            rewardedAd.Show((Reward reward) =>
////            {
////                Debug.Log("User earned reward: " + reward.Type + " " + reward.Amount);
////                HandleUserEarnedReward();
////            });
////        }
////        else
////        {
////            Debug.LogWarning("Rewarded ad not ready yet. Reloading...");
////            LoadRewardedAd();
////        }
////    }

////    private void HandleUserEarnedReward()
////    {
////        if (shootArrow != null && pendingReward > 0)
////        {
////            shootArrow.AddArrows(pendingReward);
////            pendingReward = 0;
////            Hide();
////        }
////        else
////        {
////            Debug.LogWarning("ShootArrow reference missing — did you call Initialize()?");
////        }
////    }

////    public void Show(int currentScore)
////    {
////        if (scoreText != null)
////            scoreText.text = $"Score: {currentScore}";

////        int best = PlayerPrefs.GetInt("BestScore", 0);
////        if (currentScore > best)
////        {
////            best = currentScore;
////            PlayerPrefs.SetInt("BestScore", best);
////            PlayerPrefs.Save();
////        }

////        if (bestScoreText != null)
////            bestScoreText.text = $"Best: {best}";

////        gameObject.SetActive(true);
////        isVisible = true;

////        // In case ad finished loading while popup was closed
////        SetButtonsInteractable(isAdReady);
////    }

////    public void Hide()
////    {
////        if (!isVisible) return;
////        gameObject.SetActive(false);
////        isVisible = false;
////    }

////    private void OnQuitPressed()
////    {
////        Time.timeScale = 1f;
////        SceneManager.LoadScene("Main Menu");
////    }
////}



//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using TMPro;

//public class OutOfArrowsUI : MonoBehaviour
//{
//    [Header("UI References")]
//    [SerializeField] private TextMeshProUGUI scoreText;
//    [SerializeField] private TextMeshProUGUI bestScoreText;
//    [SerializeField] private Button quitButton;
//    [SerializeField] private Button add50Button;
//    [SerializeField] private Button add100Button;
//    [SerializeField] private Button add200Button;

//    private ShootArrow shootArrow;
//    private int pendingReward = 0;
//    private bool isVisible = false;

//    private void Awake()
//    {
//        gameObject.SetActive(false);

//        quitButton.onClick.AddListener(OnQuitPressed);
//        add50Button.onClick.AddListener(() => OnWatchAdPressed(50));
//        add100Button.onClick.AddListener(() => OnWatchAdPressed(100));
//        add200Button.onClick.AddListener(() => OnWatchAdPressed(200));

//        shootArrow = FindFirstObjectByType<ShootArrow>();
//    }

//    public void Initialize(ShootArrow shooter)
//    {
//        shootArrow = shooter;
//    }

//    private void OnWatchAdPressed(int arrowsToAdd)
//    {
//        pendingReward = arrowsToAdd;

//        if (RewardedAdService.IsAdReady)
//        {

//            RewardedAdService.ShowRewardedAd(
//                onRewardEarned: HandleUserEarnedReward,
//                onAdNotReady: () => {
//                    Debug.Log("Ad not ready when clicked; reloading...");
//                    RewardedAdService.LoadRewardedAd();
//                },
//                onAdClosed: () =>
//                {
//                    // lock input briefly to prevent the leftover tap from shooting
//                    if (shootArrow != null)
//                    {
//                        shootArrow.LockInputForSeconds(0.5f);
//                    }
//                }
//            );
//        }
//        else
//        {
//            Debug.Log("Ad not ready yet — attempting to load now...");
//            RewardedAdService.LoadRewardedAd();
//        }
//    }

//    private void HandleUserEarnedReward()
//    {
//        if (shootArrow != null && pendingReward > 0)
//        {
//            shootArrow.AddArrows(pendingReward);
//            pendingReward = 0;
//            Hide();
//        }
//    }

//    public void Show(int currentScore)
//    {
//        if (scoreText != null)
//            scoreText.text = $"Score: {currentScore}";

//        int best = PlayerPrefs.GetInt("BestScore", 0);
//        if (currentScore > best)
//        {
//            best = currentScore;
//            PlayerPrefs.SetInt("BestScore", best);
//            PlayerPrefs.Save();
//        }

//        if (bestScoreText != null)
//            bestScoreText.text = $"Best: {best}";

//        gameObject.SetActive(true);
//        isVisible = true;
//    }

//    public void Hide()
//    {
//        if (!isVisible) return;
//        gameObject.SetActive(false);
//        isVisible = false;
//    }

//    private void OnQuitPressed()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("Main Menu");
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
    [SerializeField] private Button quitButton;
    [SerializeField] private Button add50Button;
    [SerializeField] private Button add100Button;
    [SerializeField] private Button add200Button;

    private ShootArrow shootArrow;
    private int pendingReward = 0;
    private bool isVisible = false;

    private void Awake()
    {
        gameObject.SetActive(false);

        quitButton.onClick.AddListener(OnQuitPressed);
        add50Button.onClick.AddListener(() => OnWatchAdPressed(50));
        add100Button.onClick.AddListener(() => OnWatchAdPressed(100));
        add200Button.onClick.AddListener(() => OnWatchAdPressed(200));

        shootArrow = FindFirstObjectByType<ShootArrow>();

        // Start with buttons disabled until ad is ready
        SetButtonsInteractable(false);
    }

    private void Update()
    {
        // Keep checking if ad becomes ready, then enable buttons
        if (isVisible)
        {
            SetButtonsInteractable(RewardedAdService.IsAdReady);
        }
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
    }

    private void OnWatchAdPressed(int arrowsToAdd)
    {
        pendingReward = arrowsToAdd;

        RewardedAdService.ShowRewardedAd(
            HandleUserEarnedReward,
            () => Debug.Log("Ad not ready yet..."),
            () =>
            {
                // optional: re-lock inputs after ad close
                if (shootArrow != null) shootArrow.LockInputForSeconds(0.5f);
            });
    }

    private void HandleUserEarnedReward()
    {
        if (shootArrow != null && pendingReward > 0)
        {
            shootArrow.AddArrows(pendingReward);
            pendingReward = 0;
            Hide();
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

        // buttons will turn on automatically when ads ready
        SetButtonsInteractable(RewardedAdService.IsAdReady);
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
        SceneManager.LoadScene("Main Menu");
    }
}
