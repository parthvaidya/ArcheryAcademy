using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ArrowPooler : MonoBehaviour
{
    public static ArrowPooler Instance;

    public GameObject arrowPrefab;
    public int poolSize = 10;

    private Queue<GameObject> arrowPool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this; // singleton for easy access
    }

    void Start()
    {
        // Pre-instantiate arrows
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            arrowPool.Enqueue(arrow);
        }
    }

    public GameObject GetArrow()
    {
        if (arrowPool.Count > 0)
        {
            GameObject arrow = arrowPool.Dequeue();
            arrow.SetActive(true);
            return arrow;
        }
        else
        {
            // Expand pool if needed
            GameObject arrow = Instantiate(arrowPrefab);
            return arrow;
        }
    }

    //public void ReturnArrow(GameObject arrow)
    //{
    //    arrow.SetActive(false);
    //    arrowPool.Enqueue(arrow);
    //}

    public void ReturnArrow(GameObject arrow)
    {
        // Reset arrow state
        arrow.SetActive(false);

        // Reset transform
        arrow.transform.SetParent(transform); // optional: keep hierarchy clean
        arrow.transform.localPosition = Vector3.zero;
        arrow.transform.localRotation = Quaternion.identity;

        // Reset rigidbody
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.Sleep();
        }

        // Reset any child objects (like fruits attached to arrow tips)
        foreach (Transform child in arrow.transform)
        {
            if (child.CompareTag("Fruit"))  // or whatever tag your fruits use
            {
                Destroy(child.gameObject); // remove stuck fruit
            }
        }

        arrowPool.Enqueue(arrow);
    }


}
