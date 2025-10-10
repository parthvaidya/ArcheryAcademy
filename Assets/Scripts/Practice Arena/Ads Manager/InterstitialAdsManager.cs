using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static InterstitialAdsManager Instance;

#if UNITY_IOS
    private string adUnitId = "Interstitial_iOS";
#elif UNITY_ANDROID
    private string adUnitId = "Interstitial_Android";
#else
    private string adUnitId = null;
#endif

    private bool isAdReady = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadAd();
    }

    public void LoadAd()
    {
        isAdReady = false;
        Advertisement.Load(adUnitId, this);
    }

    public void ShowAd()
    {
        if (isAdReady)
        {
            Advertisement.Show(adUnitId, this);
            isAdReady = false;
        }
        else
        {
            Debug.Log("[UnityAds] Interstitial not ready, reloading...");
            LoadAd();
        }
    }

    public void OnUnityAdsAdLoaded(string id) => isAdReady = true;
    public void OnUnityAdsFailedToLoad(string id, UnityAdsLoadError error, string msg)
    {
        Debug.LogWarning($"Interstitial load fail: {error} - {msg}");
        isAdReady = false;
    }
    public void OnUnityAdsShowFailure(string id, UnityAdsShowError error, string msg) => Debug.LogWarning($"Interstitial show fail: {msg}");
    public void OnUnityAdsShowStart(string id) { }
    public void OnUnityAdsShowClick(string id) { }
    public void OnUnityAdsShowComplete(string id, UnityAdsShowCompletionState state)
    {
        LoadAd(); // reload for next time
    }
}