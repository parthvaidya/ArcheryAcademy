using UnityEngine;

public class BowScript : MonoBehaviour
{

    public Vector2 direction;
    public float maxForce = 200f;
    public float currentForce;

    public GameObject PointPrefab;
    public GameObject[] Points;
    public int numberOfPoints;

    private Vector2 bowPos;
    public float forceMultiplier = 5f;
    public float step = 0.05f;

    void Start()
    {
        Points = new GameObject[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            Points[i] = Instantiate(PointPrefab, transform.position, Quaternion.identity);
            Points[i].SetActive(false); // hide at start
        }

        bowPos = transform.position;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            // drag direction is opposite
            direction = bowPos - touchPos;

            // pull distance = power
            currentForce = Mathf.Clamp(direction.magnitude * forceMultiplier, 0, maxForce);

            // face in shooting direction
            FaceForward();

            

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                ShowDots(true);

                // More dots based on force
                int activeDots = Mathf.Clamp(Mathf.RoundToInt(currentForce * 0.2f), 1, numberOfPoints);

                for (int i = 0; i < numberOfPoints; i++)
                {
                    if (i < activeDots)
                    {
                        Points[i].SetActive(true);
                        Points[i].transform.position = PointsPosition(i * step); // step = 0.02f
                    }
                    else
                    {
                        Points[i].SetActive(false);
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                ShowDots(false);
            }
        }
    }

    void FaceForward()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //  Clamp angle between limits
        float clampedAngle = Mathf.Clamp(angle, -10f, 80f); // adjust as you like

        transform.rotation = Quaternion.Euler(0, 0, clampedAngle);
    }

    Vector2 PointsPosition(float t)
    {
        Vector2 currentPointPos = bowPos +
            (direction.normalized * currentForce * t) +
            0.5f * Physics2D.gravity * (t * t);

        return currentPointPos;
    }

    public void ShowDots(bool show)
    {
        foreach (GameObject point in Points)
        {
            point.SetActive(show);
        }
    }


}
