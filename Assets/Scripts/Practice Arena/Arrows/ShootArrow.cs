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
            totalArrows = 100; // free arrows first time
            SessionManager.Instance.MarkFirstSessionComplete(); // mark session done after use
        }



        if (bowScript == null)
            bowScript = GetComponent<BowScript>();

        if (outOfArrowsUI != null)
        {
            outOfArrowsUI.Initialize(this);
            outOfArrowsUI.Hide(); // keep hidden until needed
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
            if (inputLockTimer <= 0f)
                inputLocked = false;
            else
                return; // skip Update entirely while locked
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (IsTouchOverUI(touch)) return;

            if (touch.phase == TouchPhase.Ended)
            {
                Shoot();
            }
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

        if (totalArrows <= 0)
        {
            // already no arrows, just show popup
            ShowOutOfArrowsPopup();
            return;
        }

        GameObject arrow = ArrowPooler.Instance.GetArrow();
        if (arrow == null) return;

        arrow.transform.position = transform.position;
        arrow.transform.rotation = transform.rotation;

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.linearVelocity = transform.right * bowScript.currentForce;
        bowScript.ShowDots(false);

        totalArrows--;
        UpdateArrowUI();

        // popup as soon as arrows reach 0
        if (totalArrows == 0)
        {
            ShowOutOfArrowsPopup();
        }
    }

    void ShowOutOfArrowsPopup()
    {
        if (outOfArrowsUI != null)
        {
            int finalScore = 0;

            //  Safely fetch score from GameManager or ScoreSystem
            if (GameManager.Instance != null)
            {
                finalScore = GameManager.Instance.GetScore();
            }
            else
            {
                Debug.LogWarning("GameManager not found, using fallback score 0.");
            }

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

