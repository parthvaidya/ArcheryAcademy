using TMPro;
using UnityEngine;
using System.Collections;

public class ComboSystem
{
    private readonly TextMeshProUGUI comboText;
    private readonly float comboResetTime;
    private readonly float textDisplayTime;
    private readonly float floatUpDistance;
    private readonly float fadeOutDuration;

    private int currentCombo = 0;
    private int missedFruits = 0;
    private float lastHitTime = 0f;
    private Coroutine textRoutine;

    private readonly MonoBehaviour coroutineHost;

    public ComboSystem(TextMeshProUGUI uiText, float resetTime, float displayTime, float floatDist, float fadeDuration)
    {
        comboText = uiText;
        comboResetTime = resetTime;
        textDisplayTime = displayTime;
        floatUpDistance = floatDist;
        fadeOutDuration = fadeDuration;

        if (comboText != null)
            comboText.gameObject.SetActive(false);

        // use GameManager to run coroutines
        coroutineHost = GameManager.Instance;
    }

    public void Update()
    {
        if (currentCombo > 0 && Time.time - lastHitTime > comboResetTime)
            currentCombo = 0;
    }

    public void RegisterHit()
    {
        currentCombo++;
        missedFruits = 0;
        lastHitTime = Time.time;

        // Light vibration for every hit
        

        string message = null;

        switch (currentCombo)
        {
            case 3:
                message = "3x Streak!";
                VibrateStrong(); // strong vibration only on combo streaks
                break;

            case 5:
                message = "5x Combo!";
                VibrateStrong();
                break;

            case 10:
                message = "10x Master Streak!";
                VibrateStrong();
                break;

            default:
                // no strong vibration for normal hits
                break;
        }

        if (message != null)
            ShowComboMessage(message);
    }

    public void RegisterMiss()
    {
        missedFruits++;
        if (missedFruits >= 5)
        {
            ShowComboMessage(" Missed 5 fruits! Focus!");
            missedFruits = 0;
        }
    }

    private void ShowComboMessage(string message)
    {
        if (comboText == null) return;
        if (textRoutine != null)
            coroutineHost.StopCoroutine(textRoutine);

        textRoutine = coroutineHost.StartCoroutine(AnimateText(message));
    }

    private IEnumerator AnimateText(string message)
    {
        comboText.gameObject.SetActive(true);
        comboText.text = message;

        comboText.alpha = 1f;
        comboText.transform.localScale = Vector3.one * 0.7f;
        Vector3 startPos = comboText.rectTransform.localPosition;
        Vector3 endPos = startPos + new Vector3(0f, floatUpDistance, 0f);

        float t = 0f;
        while (t < 0.2f)
        {
            comboText.transform.localScale = Vector3.Lerp(Vector3.one * 0.7f, Vector3.one * 1.2f, t / 0.2f);
            t += Time.deltaTime;
            yield return null;
        }

        comboText.transform.localScale = Vector3.one;

        float elapsed = 0f;
        Color c = comboText.color;

        while (elapsed < textDisplayTime)
        {
            float normalized = elapsed / textDisplayTime;
            comboText.rectTransform.localPosition = Vector3.Lerp(startPos, endPos, normalized);

            if (elapsed > textDisplayTime - fadeOutDuration)
            {
                float fadeT = (elapsed - (textDisplayTime - fadeOutDuration)) / fadeOutDuration;
                c.a = Mathf.Lerp(1f, 0f, fadeT);
                comboText.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        comboText.gameObject.SetActive(false);
        comboText.rectTransform.localPosition = startPos;
    }

    // 
    private void VibrateLight()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate(); // TODO: replace with small pulse for Android haptics
#endif
    }

    // 
    private void VibrateStrong()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate(); // TODO: replace with stronger pattern if using advanced API
#endif
    }
}
