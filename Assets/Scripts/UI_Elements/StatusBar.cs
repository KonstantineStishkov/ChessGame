using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] GameObject Instance;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI text;

    [Header("Settings")]
    [SerializeField] float SecondsUntilFade;

    private float lastMessageTime;

    public void Update()
    {
        if(Time.realtimeSinceStartup - lastMessageTime > SecondsUntilFade)
            Instance.SetActive(false);
    }
    public void ShowMessage(MessageType type, string message)
    {
        Instance.SetActive(true);
        SetMessageColor(type);
        text.text = message;
    }

    private void SetMessageColor(MessageType type)
    {
        lastMessageTime = Time.realtimeSinceStartup;
        switch (type)
        {
            case MessageType.Info:
                image.color = Color.green;
                break;
            case MessageType.Warning:
                image.color = Color.yellow;
                break;
            case MessageType.Error:
                image.color = Color.red;
                break;
            default:
                break;
        }
    }
}
