using UnityEngine;

public class arrowScript : MonoBehaviour
{

    
    Rigidbody2D rb;
    bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!hasHit)
        {
            trackMovement();
        }
    }

    void trackMovement()
    {
        Vector2 direction = rb.linearVelocity;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        hasHit = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (col.gameObject.CompareTag("Fruit"))
        {
            // Disable the fruit (returns to pool automatically via Fruit script Update check)
            col.gameObject.SetActive(false);

            // (Optional) Add effects
            Debug.Log("Arrow hit fruit: " + col.gameObject.name);
            //Invoke(nameof(ReturnToPool), 0.1f);
            ArrowPooler.Instance.ReturnArrow(gameObject);
            return;
        }

        // After some delay, return arrow to pool
        Invoke(nameof(ReturnToPool), 1.5f);
    }

    void OnEnable()
    {
        hasHit = false; // reset when reused
    }

    void ReturnToPool()
    {
        ArrowPooler.Instance.ReturnArrow(gameObject);
    }
}
