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
    [SerializeField] private GameObject outOfArrowsPrefab;
    [SerializeField] private Canvas canvas;

    private OutOfArrowsUI outOfArrowsUI;

    private bool inputLocked = false;
    private float inputLockTimer = 0f;

    private void Start()
    {
        if (bowScript == null)
            bowScript = GetComponent<BowScript>();

        // instantiate popup under canvas
        if (outOfArrowsPrefab != null)
        {
            if (canvas != null)
            {
                GameObject instance = Instantiate(outOfArrowsPrefab, canvas.transform, false);
                outOfArrowsUI = instance.GetComponent<OutOfArrowsUI>();

                if (outOfArrowsUI == null)
                {
                    Debug.LogError("OutOfArrows prefab is missing OutOfArrowsUI script!");
                }
                else
                {
                    // Inject this ShootArrow reference into the popup
                    outOfArrowsUI.Initialize(this);
                    outOfArrowsUI.Hide();
                }
            }
            else
            {
                Debug.LogError("Canvas not assigned in ShootArrow!");
            }
        }
        else
        {
            Debug.LogWarning("OutOfArrows prefab not assigned in ShootArrow!");
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
        // stop if no arrows left
        if (totalArrows <= 0)
        {
            ShowOutOfArrowsPopup();
            return;
        }

        if (bowScript == null || ArrowPooler.Instance == null) return;

        GameObject arrow = ArrowPooler.Instance.GetArrow();
        if (arrow == null) return;

        arrow.transform.position = transform.position;
        arrow.transform.rotation = transform.rotation;

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // apply shooting force
        rb.linearVelocity = transform.right * bowScript.currentForce;

        // hide trajectory dots
        bowScript.ShowDots(false);

        totalArrows--;
        UpdateArrowUI();

        //  check when arrows run out
        if (totalArrows <= 0)
            ShowOutOfArrowsPopup();
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

