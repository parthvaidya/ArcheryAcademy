// DailyNotification.cs
// Requires: com.unity.mobile.notifications
// Works with iOS + Android. Zero scene wiring needed.

using System;
using System.Collections;
using UnityEngine;

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public sealed class DailyNotification : MonoBehaviour
{
    // Customize here if you ever need to localize/change copy:
    const string notifTitle = "Daily Training";
    const string notifBody = "Time to sharpen your arrows";
    const int notifHour = 9;   // 9:00 AM local time
    const int notifMinute = 0;

    // Android channel constants
    const string androidChannelId = "daily_reminders";
    const string androidChannelName = "Daily Reminders";
    const string androidChannelDesc = "Game reminder notifications";

    // iOS identifier for update/cancel
    const string iosIdentifier = "daily_9am";

    // --- BOOTSTRAP: create an invisible singleton at app launch
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot()
    {
        var go = new GameObject(nameof(DailyNotification));
        DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave;
        go.AddComponent<DailyNotification>(); // one tiny Mono in the whole app
    }

    private void Start()
    {
        StartCoroutine(InitAndSchedule());
    }

    private IEnumerator InitAndSchedule()
    {
#if UNITY_ANDROID
        SetupAndroidChannel();
        // Clean up older scheduled/delivered to avoid duplicates if app relaunches
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();

        var next9am = Next9AMLocal();
        var androidNotif = new AndroidNotification
        {
            Title = notifTitle,
            Text = notifBody,
            FireTime = next9am,
            // Repeats daily
            RepeatInterval = TimeSpan.FromDays(1),
            ShouldAutoCancel = true
            // Optionally: SmallIcon = "app_icon", LargeIcon = "app_icon"
        };

        AndroidNotificationCenter.SendNotification(androidNotif, androidChannelId);
#endif

#if UNITY_IOS
        // Request permission (alerts, sound, badge). This is async; we wait once.
        var request = new AuthorizationRequest(
            AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound,
            true);

        while (!request.IsFinished)
            yield return null;

        // Even if denied, scheduling is harmless (it just won't show).
        iOSNotificationCenter.RemoveScheduledNotification(iosIdentifier);
        iOSNotificationCenter.RemoveDeliveredNotifications();

        // Calendar trigger fires daily at 9:00 local time
        var trigger = new iOSNotificationCalendarTrigger
        {
            Hour = notifHour,
            Minute = notifMinute,
            Repeats = true
        };

        var iosNotif = new iOSNotification
        {
            Identifier = iosIdentifier,
            Title = notifTitle,
            Body = notifBody,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "daily",
            ThreadIdentifier = "daily",
            Trigger = trigger
        };

        iOSNotificationCenter.ScheduleNotification(iosNotif);
#endif
        yield break;
    }

#if UNITY_ANDROID
    private static void SetupAndroidChannel()
    {
        // Safe to call multiple times; Unity handles updates idempotently.
        var channel = new AndroidNotificationChannel
        {
            Id = androidChannelId,
            Name = androidChannelName,
            Description = androidChannelDesc,
            Importance = Importance.Default
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
#endif

    private static DateTime Next9AMLocal()
    {
        var now = DateTime.Now;
        var next = new DateTime(now.Year, now.Month, now.Day, notifHour, notifMinute, 0);
        if (next <= now) next = next.AddDays(1);
        return next;
    }
}
