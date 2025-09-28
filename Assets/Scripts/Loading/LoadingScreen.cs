using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI References")]
    public Slider progressBar;             
    public TextMeshProUGUI progressText;  

    [Header("Settings")]
    public int sceneBuildIndex = 1;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }


    //Actual Loading
    //IEnumerator LoadSceneAsync()
    //{
    //    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneBuildIndex);
    //    operation.allowSceneActivation = false;

    //    while (!operation.isDone)
    //    {

    //        float progress = Mathf.Clamp01(operation.progress / 0.9f);


    //        if (progressBar != null)
    //            progressBar.value = progress;

    //        if (progressText != null)
    //            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";


    //        if (progress >= 1f)
    //        {
    //            operation.allowSceneActivation = true;
    //        }

    //        yield return null;
    //    }
    //}


    //Fake Loading
    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneBuildIndex);
        operation.allowSceneActivation = false;

        float fakeLoadTime = 3f; // minimum seconds to show loading
        float elapsed = 0f;

        while (!operation.isDone)
        {
            elapsed += Time.deltaTime;

            // Unity loading progress (0–0.9)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Combine real progress with fake timer
            float displayProgress = Mathf.Min(progress, elapsed / fakeLoadTime);

            if (progressBar != null)
                progressBar.value = displayProgress;

            if (progressText != null)
                progressText.text = Mathf.RoundToInt(displayProgress * 100f) + "%";

            // Activate only when both real progress & fake time are done
            if (displayProgress >= 1f && progress >= 1f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    }
