using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootArrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private int totalArrows = 100;

    [Header("References")]
    [SerializeField] private BowScript bowScript;
    [SerializeField] private TextMeshProUGUI arrowCountText;
    [SerializeField] private Canvas canvas;
    [SerializeField] private OutOfArrowsUI outOfArrowsUI;

    private bool inputLocked = false;
    private float inputLockTimer = 0f;

    private void Start()
    {
        if (SessionManager.Instance != null && SessionManager.Instance.IsFirstSession)
        {
            totalArrows = 100;
            SessionManager.Instance.MarkFirstSessionComplete();
        }

        if (bowScript == null)
            bowScript = GetComponent<BowScript>();

        if (outOfArrowsUI != null)
        {
            outOfArrowsUI.Initialize(this);
            outOfArrowsUI.Hide();
        }
        else
        {
            Debug.LogError("OutOfArrows UI not assigned in inspector!");
        }

        UpdateArrowUI();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        if (inputLocked)
        {
            inputLockTimer -= Time.deltaTime;
            if (inputLockTimer <= 0f) inputLocked = false;
            else return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (IsTouchOverUI(touch)) return;

            if (touch.phase == TouchPhase.Ended)
                Shoot();
        }
    }

    public void LockInputForSeconds(float duration)
    {
        inputLocked = true;
        inputLockTimer = duration;
    }

    void Shoot()
    {
        if (bowScript == null || ArrowPooler.Instance == null) return;
        if (totalArrows <= 0) { ShowOutOfArrowsPopup(); return; }

        GameObject arrow = ArrowPooler.Instance.GetArrow();
        if (arrow == null) return;

        arrow.transform.position = bowScript.arrowSpawnPoint.position;
        arrow.transform.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(bowScript.cachedAimDirection.y, bowScript.cachedAimDirection.x) * Mathf.Rad2Deg);

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.linearVelocity = bowScript.cachedAimDirection * bowScript.currentForce;

        bowScript.ShowDots(false);

        totalArrows--;
        UpdateArrowUI();

        if (totalArrows == 0)
            ShowOutOfArrowsPopup();
    }

    void ShowOutOfArrowsPopup()
    {
        if (outOfArrowsUI != null)
        {
            int finalScore = GameManager.Instance != null ? GameManager.Instance.GetScore() : 0;
            outOfArrowsUI.Show(finalScore);
        }
        else
        {
            Debug.LogWarning("OutOfArrows UI not assigned!");
        }
    }

    private bool IsTouchOverUI(Touch touch)
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }

    void UpdateArrowUI()
    {
        if (arrowCountText != null)
            arrowCountText.text = $"{totalArrows}";
    }

    public void AddArrows(int amount)
    {
        totalArrows += amount;
        UpdateArrowUI();

        if (outOfArrowsUI != null)
            outOfArrowsUI.Hide();
    }
}
