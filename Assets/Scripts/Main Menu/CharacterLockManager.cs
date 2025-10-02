
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLockManager : MonoBehaviour
{
    private int[] unlocked; // 0 = locked, 1 = unlocked

    void Start()
    {
        int childCount = transform.childCount;
        unlocked = new int[childCount];

        
        for (int i = 0; i < childCount; i++)
            unlocked[i] = (i == 0) ? 1 : 0;

        ApplyLockVisuals();
    }

    void ApplyLockVisuals()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Image img = child.GetComponent<Image>();
            Transform lockTextObj = child.Find("LockText"); // find by name
            TextMeshProUGUI lockText = lockTextObj ? lockTextObj.GetComponent<TextMeshProUGUI>() : null;

            if (img != null)
            {
                if (unlocked[i] == 0) // Locked
                {
                    img.color = new Color(0.4f, 0.4f, 0.4f, 1f); // greyed out
                    if (lockText != null) lockText.gameObject.SetActive(true);
                }
                else // Unlocked
                {
                    img.color = Color.white;
                    if (lockText != null) lockText.gameObject.SetActive(false);
                }
            }
        }
    }

    public bool IsUnlocked(int index)
    {
        if (index < 0 || index >= unlocked.Length) return false;
        return unlocked[index] == 1;
    }

    public void Unlock(int index)
    {
        if (index < 0 || index >= unlocked.Length) return;
        unlocked[index] = 1;
        ApplyLockVisuals();
    }
}
