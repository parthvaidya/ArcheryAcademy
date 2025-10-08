using UnityEngine;

public class ClosePopup : MonoBehaviour
{
    public void Close()
    {
        // Go upwards until we reach the prefab root
        Transform t = transform;
        while (t.parent != null && t.parent.GetComponent<Canvas>() == null)
        {
            t = t.parent;
        }
        //Destroy(t.gameObject); // destroys only popup prefab

        t.gameObject.SetActive(false);
    }
}