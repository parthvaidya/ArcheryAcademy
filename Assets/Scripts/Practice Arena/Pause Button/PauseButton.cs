using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [Tooltip("Reference to the Pause Popup panel in the scene.")]
    public GameObject pausePopup;

    [Tooltip("The Pause Button itself.")]
    public Button pauseButton;

    [Tooltip("Reference to the arrow shooting script to disable while paused.")]
    public ShootArrow shootArrow;

    private bool isPaused = false;

    private void Awake()
    {
        if (pausePopup != null)
            pausePopup.SetActive(false);
    }

    public void OnPausePressed()
    {
        if (isPaused) return;

        isPaused = true;
        pausePopup.SetActive(true);
        pauseButton.gameObject.SetActive(false);

        Time.timeScale = 0f;

        // disable arrow shooting while paused
        if (shootArrow != null)
            shootArrow.enabled = false;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePopup.SetActive(false);
        pauseButton.gameObject.SetActive(true);

        Time.timeScale = 1f;

        // re-enable arrow shooting
        if (shootArrow != null)
            shootArrow.enabled = true;
    }
}