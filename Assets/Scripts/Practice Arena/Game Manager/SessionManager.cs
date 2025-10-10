using UnityEngine;
using System;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    private const string FirstLoginKey = "FirstLoginTime";
    private const string FirstSessionKey = "FirstSessionCompleted";

    public bool IsFirstSession { get; private set; }
    public bool ShouldShowAds { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        EvaluateSession();
    }

    private void EvaluateSession()
    {
        // First ever play?
        if (!PlayerPrefs.HasKey(FirstLoginKey))
        {
            PlayerPrefs.SetString(FirstLoginKey, DateTime.UtcNow.ToString());
            PlayerPrefs.SetInt(FirstSessionKey, 0); // first session not completed yet
            PlayerPrefs.Save();

            IsFirstSession = true;
            ShouldShowAds = false; // free session
            Debug.Log("[SessionManager] First time player — ads disabled for this session.");
            return;
        }

        // Already played before
        IsFirstSession = false;

        // If first session is completed  allow ads
        bool firstSessionDone = PlayerPrefs.GetInt(FirstSessionKey, 0) == 1;
        ShouldShowAds = firstSessionDone;
    }


    public bool CanShowInterstitial()
    {
        float lastTime = PlayerPrefs.GetFloat("LastAdTime", -9999f);
        if (Time.realtimeSinceStartup - lastTime > 180f) // 3 min cooldown
        {
            PlayerPrefs.SetFloat("LastAdTime", Time.realtimeSinceStartup);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public void MarkFirstSessionComplete()
    {
        if (IsFirstSession)
        {
            PlayerPrefs.SetInt(FirstSessionKey, 1);
            PlayerPrefs.Save();
            IsFirstSession = false;
            ShouldShowAds = true;
            Debug.Log("[SessionManager] First session complete. Ads enabled from now.");
        }
    }
}