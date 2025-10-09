using System;
using UnityEngine;
using GoogleMobileAds.Api;

public static class RewardedAdService
{
    private static RewardedAd rewardedAd;
    private static bool isAdReady = false;
    private static DateTime lastLoadAttempt = DateTime.MinValue;
    private const float RetryDelaySeconds = 3f;

    // callback invoked when the ad was closed (per-show)
    private static Action adClosedCallback;

    public static bool IsAdReady => isAdReady;

    static RewardedAdService()
    {
        MobileAds.Initialize(initStatus => { });
        LoadRewardedAd();
    }

    public static void LoadRewardedAd()
    {
        if ((DateTime.Now - lastLoadAttempt).TotalSeconds < RetryDelaySeconds)
            return;

        lastLoadAttempt = DateTime.Now;

#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // TEST ad
#elif UNITY_IOS
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unused";
#endif

        Debug.Log("[RewardedAdService] Loading rewarded ad...");
        isAdReady = false;

        AdRequest adRequest = new AdRequest();

        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogWarning("[RewardedAdService] Failed to load rewarded ad: " + error);
                rewardedAd = null;
                isAdReady = false;
                return;
            }

            rewardedAd = ad;
            isAdReady = true;
            Debug.Log("[RewardedAdService]  Rewarded ad loaded successfully.");

            // when ad is closed, invoke the per-show callback (if any), then reload
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                try
                {
                    // call the callback that was set when ShowRewardedAd was called
                    adClosedCallback?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("[RewardedAdService] Exception in adClosedCallback: " + ex);
                }
                finally
                {
                    // clear it so it won't be reused unintentionally
                    adClosedCallback = null;
                }

                Debug.Log("[RewardedAdService] Ad closed — reloading...");
                LoadRewardedAd();
            };

            rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogWarning("[RewardedAdService] Ad failed to show: " + adError);
                // also call adClosedCallback to restore input if needed
                try { adClosedCallback?.Invoke(); } catch { }
                adClosedCallback = null;

                LoadRewardedAd();
            };
        });
    }

   
    public static void ShowRewardedAd(Action onRewardEarned, Action onAdNotReady = null, Action onAdClosed = null)
    {
        if (rewardedAd != null && isAdReady)
        {
            isAdReady = false;

            // store the ad-closed callback for the current show
            adClosedCallback = onAdClosed;

            Debug.Log("[RewardedAdService] Showing rewarded ad...");
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("[RewardedAdService] User earned reward!");
                onRewardEarned?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("[RewardedAdService] Rewarded ad not ready — reloading.");
            onAdNotReady?.Invoke();
            LoadRewardedAd();
        }
    }
}
