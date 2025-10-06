

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootArrow : MonoBehaviour
{
    private BowScript bowScript;

    [Header("Arrow Settings")]
    public int totalArrows = 100; // starting arrow count

    [Header("UI")]
    public TextMeshProUGUI arrowCountText; // assign in Inspector

    private void Start()
    {
        bowScript = GetComponent<BowScript>();
        UpdateArrowUI();
    }

    private void Update()
    {

        if (Time.timeScale == 0f) return;

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

    void Shoot()
    {
        // stop if no arrows left
        if (totalArrows <= 0)
        {
            Debug.Log("No arrows left!");
            return;
        }

        if (bowScript == null || ArrowPooler.Instance == null) return;

        // get arrow from pool
        GameObject arrow = ArrowPooler.Instance.GetArrow();
        if (arrow == null) return;

        // reset transform and physics
        arrow.transform.position = transform.position;
        arrow.transform.rotation = transform.rotation;

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // apply shooting force
        rb.linearVelocity = transform.right * bowScript.currentForce;

        // hide trajectory dots
        bowScript.ShowDots(false);

        // decrement arrow count
        totalArrows--;
        UpdateArrowUI();
    }


    private bool IsTouchOverUI(Touch touch)
    {
        // check if this touch is over any UI element
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }
    void UpdateArrowUI()
    {
        if (arrowCountText != null)
        {
            arrowCountText.text = $"{totalArrows}";
        }
    }

    // optional — call this when you reward more arrows
    public void AddArrows(int amount)
    {
        totalArrows += amount;
        UpdateArrowUI();
    }
}


