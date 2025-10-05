
using UnityEngine;
using System.Collections;

public class Fruit : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    public FruitData data;
    public bool isHit { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        isHit = false;
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
            col.enabled = false;
            StartCoroutine(EnableColliderNextFrame());
        }
    }

    IEnumerator EnableColliderNextFrame()
    {
        yield return new WaitForFixedUpdate();
        if (col != null) col.enabled = true;
    }

    public void Launch(Vector2 direction)
    {
        if (data == null) return;

        rb.AddForce(direction * data.GetRandomizedForce(), ForceMode2D.Impulse);
        rb.AddTorque(data.GetRandomTorque(), ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHit) return;
        if (!collision.gameObject.CompareTag("Arrow")) return;

        isHit = true;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(data.points);
    }

    void Update()
    {
        if (!isHit && transform.position.y < -6f)
            gameObject.SetActive(false);
    }
}