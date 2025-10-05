using UnityEngine;
using System.Collections;

public class ArrowView : MonoBehaviour
{
    public Rigidbody2D rb { get; private set; }
    public Collider2D arrowCollider { get; private set; }

    private bool hasHit;
    private ArrowController controller;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        arrowCollider = GetComponent<Collider2D>();
        controller = new ArrowController(this);
    }

    void OnEnable() => controller.OnEnable();
    void Update() => controller.Update();
    void OnCollisionEnter2D(Collision2D col) => controller.OnCollision(col);
}