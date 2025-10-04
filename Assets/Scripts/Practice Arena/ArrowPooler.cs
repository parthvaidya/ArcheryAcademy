using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ArrowPooler : MonoBehaviour
{
    //public static ArrowPooler Instance;

    //public GameObject arrowPrefab;
    //public int poolSize = 10;

    //private Queue<GameObject> arrowPool = new Queue<GameObject>();

    //void Awake()
    //{
    //    Instance = this; // singleton for easy access
    //}

    //void Start()
    //{
    //    // Pre-instantiate arrows
    //    for (int i = 0; i < poolSize; i++)
    //    {
    //        GameObject arrow = Instantiate(arrowPrefab);
    //        arrow.SetActive(false);
    //        arrowPool.Enqueue(arrow);
    //    }
    //}

    //public GameObject GetArrow()
    //{
    //    if (arrowPool.Count > 0)
    //    {
    //        GameObject arrow = arrowPool.Dequeue();
    //        arrow.SetActive(true);
    //        return arrow;
    //    }
    //    else
    //    {
    //        // Expand pool if needed
    //        GameObject arrow = Instantiate(arrowPrefab);
    //        return arrow;
    //    }
    //}

    //public void ReturnArrow(GameObject arrow)
    //{
    //    arrow.SetActive(false);
    //    arrowPool.Enqueue(arrow);
    //}

    public static ArrowPooler Instance;

    [Header("Addressable Settings")]
    public string arrowAddress = "Assets/Prefabs/Arrow.prefab"; //  Use simple key, not full path
    public int poolSize = 10;

    private Queue<GameObject> arrowPool = new Queue<GameObject>();
    private GameObject arrowPrefab; //  keep prefab cached
    private bool isReady = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(PreloadArrows());
    }

    IEnumerator PreloadArrows()
    {
        // Load prefab once
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(arrowAddress);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            arrowPrefab = handle.Result;

            // Pre-instantiate arrows
            for (int i = 0; i < poolSize; i++)
            {
                GameObject arrow = Instantiate(arrowPrefab);
                arrow.SetActive(false);
                arrowPool.Enqueue(arrow);
            }

            isReady = true;
        }
        else
        {
            Debug.LogError($" Failed to load Addressable: {arrowAddress}");
        }
    }

    public GameObject GetArrow()
    {
        if (!isReady)
        {
            Debug.LogWarning(" Arrows not ready yet!");
            return null;
        }

        if (arrowPool.Count > 0)
        {
            GameObject arrow = arrowPool.Dequeue();
            arrow.SetActive(true);
            return arrow;
        }
        else
        {
            // Expand pool instantly using cached prefab
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(true);
            return arrow;
        }
    }

    public void ReturnArrow(GameObject arrow)
    {
        arrow.SetActive(false);
        arrowPool.Enqueue(arrow);
    }
}
