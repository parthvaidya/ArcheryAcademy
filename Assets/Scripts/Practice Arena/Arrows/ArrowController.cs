using UnityEngine;
using System.Collections;

public class ArrowController
{
    private readonly ArrowView view;
    private Transform stuckFruit;
    private Rigidbody2D stuckFruitRb;
    private Collider2D stuckFruitCol;

    public ArrowController(ArrowView view) => this.view = view;

    public void OnEnable()
    {
        ResetState();
        view.arrowCollider.enabled = false;
        view.rb.simulated = true;
        view.StartCoroutine(EnableColliderDelayed());
    }

    IEnumerator EnableColliderDelayed()
    {
        yield return new WaitForSeconds(0.05f);
        view.arrowCollider.enabled = true;
    }

    public void Update()
    {
        if (!view.rb.simulated) return;

        TrackMovement();

        Vector3 screenPos = Camera.main.WorldToViewportPoint(view.transform.position);
        if (screenPos.x < -0.1f || screenPos.x > 1.1f || screenPos.y < -0.1f || screenPos.y > 1.1f)
        {
            GameManager.Instance?.RegisterMiss();
            ArrowPooler.Instance.ReturnArrow(view.gameObject);
        }
    }

    private void ResetState()
    {
        view.rb.linearVelocity = Vector2.zero;
        view.rb.angularVelocity = 0f;
        view.transform.SetParent(null);
    }

    private void TrackMovement()
    {
        Vector2 dir = view.rb.linearVelocity;
        if (dir.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            view.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void OnCollision(Collision2D col)
    {
        if (stuckFruit != null) return; // already hit

        if (col.gameObject.CompareTag("Fruit"))
        {
            HandleFruitHit(col);
        }
        else
        {
            GameManager.Instance?.RegisterMiss();
            view.rb.linearVelocity = Vector2.zero;
            view.arrowCollider.enabled = false;
            view.StartCoroutine(ReturnAfter(1.5f));
        }
    }

    private void HandleFruitHit(Collision2D col)
    {
        GameManager.Instance?.RegisterHit();

        stuckFruit = col.transform;
        stuckFruitRb = stuckFruit.GetComponent<Rigidbody2D>();
        stuckFruitCol = stuckFruit.GetComponent<Collider2D>();

        if (stuckFruitRb != null)
        {
            stuckFruitRb.bodyType = RigidbodyType2D.Kinematic;
            stuckFruitRb.linearVelocity = view.rb.linearVelocity;
        }

        if (stuckFruitCol != null)
            stuckFruitCol.enabled = false;

        stuckFruit.SetParent(view.transform, true);
        view.arrowCollider.enabled = false;
        view.rb.linearVelocity *= 0.7f;

        view.StartCoroutine(DetachAndReturn(1.2f));
    }

    IEnumerator DetachAndReturn(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (stuckFruit != null)
        {
            stuckFruit.SetParent(null, true);
            if (stuckFruitRb != null)
            {
                stuckFruitRb.bodyType = RigidbodyType2D.Dynamic;
                stuckFruitRb.linearVelocity = Vector2.zero;
                stuckFruitRb.gravityScale = 1f;
            }
            if (stuckFruitCol != null)
                stuckFruitCol.enabled = true;
            stuckFruit.gameObject.SetActive(false);
        }
        ArrowPooler.Instance.ReturnArrow(view.gameObject);
    }

    IEnumerator ReturnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        ArrowPooler.Instance.ReturnArrow(view.gameObject);
    }
}
