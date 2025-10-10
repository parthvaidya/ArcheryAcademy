using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadPracticeArena()
    {

        if (SessionManager.Instance != null && SessionManager.Instance.ShouldShowAds)
        {
            // Show interstitial
            if (InterstitialAdsManager.Instance != null)
                InterstitialAdsManager.Instance.ShowAd();
        }
        SceneManager.LoadScene("Practice Arena");
    }

    public void LoadPlayArea()
    {
        SceneManager.LoadScene("Play Area");
    }
}
