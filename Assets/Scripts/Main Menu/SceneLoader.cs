using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadPracticeArena()
    {
        SceneManager.LoadScene("Practice Arena");
    }

    public void LoadPlayArea()
    {
        SceneManager.LoadScene("Play Area");
    }
}
