using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using Unity.Notifications.Android;

public class AlienNotifications : MonoBehaviour
{
    void Start()
    {
        CreatePopUpChannel();
        PlanPopUp("FISH-HEAD: ���? �� ��� ��������?", "�������� �� ��������������� ���������� � ����� ������! �� �� ������� ����������.", "alien_1", 0.1);
        PlanPopUp("Alien-�������: ��� ����� ����������?", "����� �� ������ � ����� �������� �������? ������������� �������� ��� ���������� ������� �����..", "alien_0", 0.14);
        PlanPopUp("MC DUKE: ��...", "", "alien_2", 0.18);
    }

    void Update()
    {
        
    }

    public void CreatePopUpChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Alien Corp",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

    }

    public void PlanPopUp(string title, string text, string icon, double time)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.FireTime = System.DateTime.Now.AddMinutes(time);
        notification.LargeIcon = icon;
        notification.SmallIcon = "icon";

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
}
