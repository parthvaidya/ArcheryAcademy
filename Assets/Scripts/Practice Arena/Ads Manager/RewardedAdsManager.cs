using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class RewardedAdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static RewardedAdsManager Instance;
    public bool IsAdReady => isAdReady;

#if UNITY_IOS
    private string adUnitId = "Rewarded_iOS";
#elif UNITY_ANDROID
    private string adUnitId = "Rewarded_Android";
#else
    private string adUnitId = null;
#endif

    private Action onRewardCallback;
    private bool isAdReady = false;  //  track readiness

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
        LoadAd();
    }

    public void LoadAd()
    {
        Debug.Log("[UnityAds] Loading Ad: " + adUnitId);
        isAdReady = false;
        Advertisement.Load(adUnitId, this);
    }

    public void ShowAd(Action onReward)
    {
        if (isAdReady)   //  use our flag instead of Advertisement.IsReady
        {
            onRewardCallback = onReward;
            Debug.Log("[UnityAds] Showing Ad...");
            Advertisement.Show(adUnitId, this);
            isAdReady = false; // reset until next load
        }
        else
        {
            Debug.LogWarning("[UnityAds] Ad not ready yet, reloading...");
            LoadAd();
        }
    }

   
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("[UnityAds] Ad Loaded: " + adUnitId);
        isAdReady = true; // 
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"[UnityAds] Failed to load Ad Unit {adUnitId}: {error} - {message}");
        isAdReady = false;
    }

   
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"[UnityAds] Failed to show Ad Unit {adUnitId}: {error} - {message}");
        isAdReady = false;
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("[UnityAds] Rewarded Ad Completed — grant reward!");
            onRewardCallback?.Invoke();
        }
        else
        {
            Debug.Log("[UnityAds] Ad closed without full watch — no reward");
        }

        // reload for next time
        LoadAd();
    }
}
