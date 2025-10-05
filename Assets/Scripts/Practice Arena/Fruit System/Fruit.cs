using System.Collections;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    
    private Rigidbody2D rb;
    private Collider2D col;
    public int points = 10;
    [HideInInspector] public bool isHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    //void OnEnable()
    //{
    //    // Reset physics every time fruit is reused
    //    if (rb != null)
    //    {
    //        rb.linearVelocity = Vector2.zero;
    //        rb.angularVelocity = 0f;
    //        rb.gravityScale = 1f;
    //        rb.bodyType = RigidbodyType2D.Dynamic;
    //        rb.simulated = true;
    //    }

    //    if (col != null) col.enabled = true;

    //    isHit = false;
    //}

    void OnEnable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 1f;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
        }

        if (col != null)
        {
            col.enabled = false; // <-- temporarily disable collider
            StartCoroutine(EnableColliderNextFrame());
        }

        isHit = false;
    }

    IEnumerator EnableColliderNextFrame()
    {
        yield return new WaitForFixedUpdate();
        if (col != null)
            col.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHit) return;

        if (collision.gameObject.CompareTag("Arrow"))
        {
            isHit = true;

            //if (ScoreManager.Instance != null)
            //    ScoreManager.Instance.AddScore(points);
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(points);

            // optional: spawn local hit VFX (arrow typically handles explosion), etc.
        }
    }

    void Update()
    {
        // if missed and fell below screen, recycle
        if (!isHit && transform.position.y < -6f)
        {
            gameObject.SetActive(false);
        }
    }

}
