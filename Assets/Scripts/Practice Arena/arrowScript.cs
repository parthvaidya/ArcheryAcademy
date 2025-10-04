using System.Collections;
using UnityEngine;

public class arrowScript : MonoBehaviour
{


    Rigidbody2D rb;
    bool hasHit = false;

    // info about the fruit that gets stuck
    Transform stuckFruit;
    Rigidbody2D stuckFruitRb;
    Collider2D stuckFruitCol;

    Collider2D arrowCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        arrowCollider = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        // Reset state when reused from pool
        hasHit = false;
        stuckFruit = null;
        stuckFruitRb = null;
        stuckFruitCol = null;

        // restore physics & collider
        if (arrowCollider != null) arrowCollider.enabled = true;
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // unparent in case it was parented previously
        transform.SetParent(null);
    }

    void Update()
    {
        if (!hasHit)
        {
            trackMovement();
        }
        else if (stuckFruit != null)
        {
            // keep fruit exactly on the arrow (transform parent already handles this,
            // but keep here in case you want additional offsets)
            // stuckFruit.position = transform.position;
        }
    }

    void trackMovement()
    {
        Vector2 direction = rb.linearVelocity;
        if (direction.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (hasHit) return; // only first hit matters

        if (col.gameObject.CompareTag("Fruit"))
        {
            hasHit = true;

            // cache references
            stuckFruit = col.transform;
            stuckFruitRb = stuckFruit.GetComponent<Rigidbody2D>();
            stuckFruitCol = stuckFruit.GetComponent<Collider2D>();

            // Prevent the fruit from participating in physics and collisions:
            if (stuckFruitRb != null)
            {
                // match velocities to avoid visual jump
                stuckFruitRb.linearVelocity = rb.linearVelocity;
                stuckFruitRb.angularVelocity = 0f;

                // make kinematic so it's safe to parent and not influenced by physics
                stuckFruitRb.bodyType = RigidbodyType2D.Kinematic;
            }

            if (stuckFruitCol != null)
            {
                // disable collider so it doesn't hit other fruits
                stuckFruitCol.enabled = false;
            }

            // parent fruit to arrow so it follows precisely
            stuckFruit.SetParent(transform, true);

            // disable arrow collider so the arrow doesn't bounce or collide further
            if (arrowCollider != null) arrowCollider.enabled = false;

            // optionally slow arrow slightly so it looks like it's dragging
            rb.linearVelocity *= 0.7f;

            // schedule detach + return (adjust delay as you like)
            StartCoroutine(DetachAndReturnCoroutine(1.2f));
        }
        else
        {
            // hit something else (ground/wall). Stop and return after short delay.
            hasHit = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            if (arrowCollider != null) arrowCollider.enabled = false;
            StartCoroutine(ReturnArrowAfter(1.5f));
        }
    }

    IEnumerator DetachAndReturnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (stuckFruit != null)
        {
            // unparent
            stuckFruit.SetParent(null, true);

            // restore fruit physics & collider so it will behave normally the next time it's reused
            if (stuckFruitRb != null)
            {
                stuckFruitRb.bodyType = RigidbodyType2D.Dynamic;
                stuckFruitRb.linearVelocity = Vector2.zero;
                stuckFruitRb.angularVelocity = 0f;
                stuckFruitRb.gravityScale = 1f;
            }

            if (stuckFruitCol != null)
            {
                stuckFruitCol.enabled = true;
            }

            // return fruit to pool (deactivate)
            stuckFruit.gameObject.SetActive(false);

            // clear references
            stuckFruit = null;
            stuckFruitRb = null;
            stuckFruitCol = null;
        }

        // return arrow to pool
        ArrowPooler.Instance.ReturnArrow(gameObject);
    }

    IEnumerator ReturnArrowAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        ArrowPooler.Instance.ReturnArrow(gameObject);
    }
}
