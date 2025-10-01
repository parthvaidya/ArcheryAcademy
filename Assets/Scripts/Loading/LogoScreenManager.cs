using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScreenManager : MonoBehaviour
{
    public void OnLogoAnimationComplete()
    {
        // Load next scene (e.g., MainMenu or Loading Screen)
        SceneManager.LoadScene("Main Menu");
    }
}
