using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject popupPrefab;   // Your popup prefab
    public Transform canvasParent;   // Where the popup should appear (usually your Canvas)

    private GameObject currentPopup;

    public void OnSettingsPressed()
    {
        if (currentPopup == null) // only open if not already open
        {
            currentPopup = Instantiate(popupPrefab, canvasParent);
        }
        else
        {
            Destroy(currentPopup); // toggle off if already open
            currentPopup = null;
        }
    }
}
