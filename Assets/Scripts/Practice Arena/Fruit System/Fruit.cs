using UnityEngine;

public class Fruit : MonoBehaviour
{
    private Rigidbody2D rb;
    public int points = 10;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // Reset physics every time fruit is reused
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Arrow"))
        {
            // Arrow hit  disable fruit
            ScoreManager.Instance.AddScore(points);
            gameObject.SetActive(false);

            // (Optional: Add pop effect or particle system here)
        }
    }

    void Update()
    {
        // If fruit falls below screen, disable
        if (transform.position.y < -6f)
        {
            gameObject.SetActive(false);
        }
    }
}
