using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherListPopulator : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public RectTransform content;        // ScrollView Content (Viewport -> Content)
    public GameObject[] archerUIPrefabs; // Drag your Archer_UI prefabs here (1 or many)

    void Start()
    {
        foreach (GameObject prefab in archerUIPrefabs)
        {
            // Spawn directly under Content
            GameObject newArcher = Instantiate(prefab, content);

            // Reset its transform so it aligns properly
            RectTransform rt = newArcher.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.localScale = Vector3.one;             // keep normal scale
                rt.anchoredPosition3D = Vector3.zero;    // let layout group handle positioning
                rt.offsetMin = Vector2.zero;             // reset stretch
                rt.offsetMax = Vector2.zero;
            }
        }
    }
}
