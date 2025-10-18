using UnityEngine;

public class BowScript : MonoBehaviour
{
    public Vector2 direction;
    public float maxForce = 200f;
    public float currentForce;

    public GameObject PointPrefab;
    public GameObject[] Points;
    public int numberOfPoints;

    public float forceMultiplier = 5f;
    public float step = 0.05f;

    private float holdTime = 0f;
    private bool isHolding = false;

    //[SerializeField] private Animator animator;
    [SerializeField] public Transform arrowSpawnPoint;
    [SerializeField] public Transform trajectoryStartPoint;
    [SerializeField] private Transform chestBone;

    // shared aim direction
    [HideInInspector] public Vector2 cachedAimDirection;

    void Start()
    {
        Points = new GameObject[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            Points[i] = Instantiate(PointPrefab, transform.position, Quaternion.identity);
            Points[i].SetActive(false);
        }
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            // drag direction (from bow to finger)
            direction = (Vector2)(trajectoryStartPoint.position) - touchPos;
            cachedAimDirection = direction.normalized;

            // pull distance = power
            currentForce = Mathf.Clamp(direction.magnitude * forceMultiplier, 0, maxForce);

            // face character towards aim
            FaceForward();

            if (touch.phase == TouchPhase.Began)
            {
                holdTime = 0f;
                isHolding = true;
                //if (animator != null) animator.SetBool("isDrawing", true);
            }

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                holdTime += Time.deltaTime;
                if (holdTime > 2f) currentForce *= 0.98f; // slow decay if held

                ShowDots(true);

                int activeDots = Mathf.Clamp(Mathf.RoundToInt(currentForce * 0.2f), 1, numberOfPoints);
                for (int i = 0; i < numberOfPoints; i++)
                {
                    if (i < activeDots)
                    {
                        Points[i].SetActive(true);
                        Points[i].transform.position = PointsPosition(i * step);
                    }
                    else
                        Points[i].SetActive(false);
                }

                //if (animator != null)
                //    animator.SetFloat("drawPercent", currentForce / maxForce);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                ShowDots(false);
                holdTime = 0f;
                isHolding = false;
                //if (animator != null)
                //{
                //    animator.SetBool("isDrawing", false);
                //    animator.SetTrigger("release");
                //}
            }
        }
    }

    void FaceForward()
    {
        float angle = Mathf.Atan2(cachedAimDirection.y, cachedAimDirection.x) * Mathf.Rad2Deg;
        float clampedAngle = Mathf.Clamp(angle, -10f, 80f);

        if (chestBone != null)
            chestBone.localRotation = Quaternion.Euler(0, 0, clampedAngle);
    }

    //Vector2 PointsPosition(float t)
    //{
    //    Vector2 startPos = trajectoryStartPoint != null ?
    //                       (Vector2)trajectoryStartPoint.position :
    //                       (Vector2)arrowSpawnPoint.position;

    //    Vector2 velocity = cachedAimDirection * currentForce;

    //    return startPos + (velocity * t) + 0.5f * Physics2D.gravity * (t * t);
    //}

    Vector2 PointsPosition(float t)
    {
        Vector2 startPos = (Vector2)arrowSpawnPoint.position;
        Vector2 velocity = cachedAimDirection * currentForce;

        return startPos + (velocity * t) + 0.5f * Physics2D.gravity * (t * t);
    }
    public void ShowDots(bool show)
    {
        foreach (GameObject point in Points)
            point.SetActive(show);
    }
}
