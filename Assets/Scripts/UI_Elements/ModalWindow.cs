using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalWindow : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField] TextMeshProUGUI info;
    [SerializeField] TextMeshProUGUI label1;
    [SerializeField] TextMeshProUGUI input1;
    [SerializeField] TextMeshProUGUI label2;
    [SerializeField] TextMeshProUGUI input2;
    [SerializeField] TextMeshProUGUI buttonLabel1;
    [SerializeField] TextMeshProUGUI buttonLabel2;

    [Header("Components")]
    [SerializeField] GameObject Input1;
    [SerializeField] GameObject Input2;
    [SerializeField] GameObject Button1;
    [SerializeField] GameObject Button2;

    const string okMessage = "Ok";
    const string resetMessage = "Reset";
    const string exitMessage = "Exit";
    public string CallWindow(WindowType type, string message)
    {
        string result = null;

        switch (type)
        {
            case WindowType.Info:
                CallInfoWindow(message);
                Button btn = Button1.GetComponent<Button>();
                btn.onClick.AddListener(() => result = okMessage);

                while (result == null)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                return result;

            case WindowType.Select:
                CallSelectWindow(message, resetMessage, exitMessage);
                btn = Button1.GetComponent<Button>();
                btn.onClick.AddListener(() => result = resetMessage);

                Button btn2 = Button2.GetComponent<Button>();
                btn2.onClick.AddListener(() => result = exitMessage);

                while (result == null)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                return result;
        }

        return string.Empty;
    }

    private void CallInfoWindow(string message)
    {
        info.text = message;
        DeactivateAll();
        Button1.SetActive(true);
        buttonLabel1.text = okMessage;
    }
    private void CallSelectWindow(string message, string button1Message, string button2Message)
    {
        info.text = message;
        DeactivateAll();
        Button1.SetActive(true);
        buttonLabel1.text = button1Message;

        Button2.SetActive(true);
        buttonLabel2.text = button2Message;
    }

    private void DeactivateAll()
    {
        Input1.SetActive(false);
        Input2.SetActive(false);
        Button1.SetActive(false);
        Button2.SetActive(false);
    }
}
