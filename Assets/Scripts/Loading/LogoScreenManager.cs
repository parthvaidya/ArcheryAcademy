using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScreenManager : MonoBehaviour
{
   [SerializeField] private float logoDuration = 3f; // Time to show logo (in seconds)

    private void Start()
    {
        // Start a coroutine to wait and then load the next scene
        StartCoroutine(ShowLogoAndLoad());
    }

    private IEnumerator ShowLogoAndLoad()
    {
        yield return new WaitForSeconds(logoDuration);
        SceneManager.LoadScene("Main Menu"); // Make sure scene name matches exactly
    }
}
