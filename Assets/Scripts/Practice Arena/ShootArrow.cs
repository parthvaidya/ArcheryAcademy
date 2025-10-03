using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    //public GameObject Arrow;
    //private BowScript bowScript;

    //void Start()
    //{
    //    bowScript = GetComponent<BowScript>();
    //}

    //void Update()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);

    //        // shoot when finger is released
    //        if (touch.phase == TouchPhase.Ended)
    //        {
    //            Shoot();
    //        }
    //    }
    //}

    //void Shoot()
    //{
    //    if (bowScript == null) return;

    //    GameObject ArrowIns = Instantiate(Arrow, transform.position, transform.rotation);

    //    ArrowIns.GetComponent<Rigidbody2D>().linearVelocity =
    //        transform.right * bowScript.currentForce;

    //    //  Hide dots after shooting
    //    bowScript.ShowDots(false);
    //}

    private BowScript bowScript;

    void Start()
    {
        bowScript = GetComponent<BowScript>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // shoot when finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        if (bowScript == null || ArrowPooler.Instance == null) return;

        // Get arrow from pool instead of Instantiate
        GameObject arrow = ArrowPooler.Instance.GetArrow();

        // reset transform
        arrow.transform.position = transform.position;
        arrow.transform.rotation = transform.rotation;

        // reset physics
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // shoot with calculated force
        rb.linearVelocity = transform.right * bowScript.currentForce;

        // Hide dots after shooting
        bowScript.ShowDots(false);
    }

}
