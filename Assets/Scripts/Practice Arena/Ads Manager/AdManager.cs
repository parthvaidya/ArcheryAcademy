using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private RewardedAd rewardedAd;
    private bool isAdReady = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads initialized.");
            LoadRewardedAd();
        });
    }

    public bool IsRewardedAdReady => isAdReady;

    public void LoadRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // test id
#elif UNITY_IOS
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unused";
#endif

        Debug.Log("Loading rewarded ad...");
        isAdReady = false;

        AdRequest request = new AdRequest();
        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Rewarded Ad failed to load: " + error);
                return;
            }

            rewardedAd = ad;
            isAdReady = true;
            Debug.Log("Rewarded Ad loaded.");

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Ad closed, reloading...");
                LoadRewardedAd();
            };
        });
    }

    public void ShowRewardedAd(Action onRewardEarned, Action onAdClosed = null)
    {
        if (rewardedAd != null && isAdReady)
        {
            isAdReady = false;
            rewardedAd.Show(reward =>
            {
                Debug.Log("User earned reward: " + reward.Type + " " + reward.Amount);
                onRewardEarned?.Invoke();
            });

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                onAdClosed?.Invoke();
                LoadRewardedAd();
            };
        }
        else
        {
            Debug.LogWarning("Ad not ready yet, loading again...");
            LoadRewardedAd();
        }
    }
}
