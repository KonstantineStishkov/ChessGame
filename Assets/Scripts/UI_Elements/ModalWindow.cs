using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalWindow : MonoBehaviour
{
    [Header("Main Object")]
    [SerializeField] GameObject MainWindow;

    [Header("Text Fields")]
    [SerializeField] TextMeshProUGUI info;
    [SerializeField] TextMeshProUGUI label1;
    [SerializeField] TextMeshProUGUI input1;
    [SerializeField] TextMeshProUGUI label2;
    [SerializeField] TMP_InputField input2;
    [SerializeField] TextMeshProUGUI buttonLabel1;
    [SerializeField] TextMeshProUGUI buttonLabel2;

    [Header("Components")]
    [SerializeField] GameObject Input1;
    [SerializeField] GameObject Input2;
    [SerializeField] GameObject Button1;
    [SerializeField] GameObject Button2;

    public string Field1 { get; private set; }
    public string Field2 { get; private set; }

    const string okMessage = "Ok";
    const string exitMessage = "Exit";
    public void CallWindow(WindowType type, string message, UnityAction action1 = null, UnityAction action2 = null, string actionMessage1 = okMessage, string actionMessage2 = exitMessage)
    {
        MainWindow.SetActive(true);
        var isit = MainWindow.activeInHierarchy;

        switch (type)
        {
            case WindowType.Info:
                CallInfoWindow(message);
                UnityAction action = action1 == null ? () => MainWindow.SetActive(false) : action1;
                Button1.GetComponent<Button>().onClick.AddListener(action);
                return;

            case WindowType.Select:
                CallSelectWindow(message, actionMessage1, actionMessage2);
                Button1.GetComponent<Button>().onClick.AddListener(action1);
                Button2.GetComponent<Button>().onClick.AddListener(action2);
                return;

            case WindowType.DoubleInput:
                CallLoginWindow(message);
                Button1.GetComponent<Button>().onClick.AddListener(() => OnInputButtonClick(action1));
                Button2.GetComponent<Button>().onClick.AddListener(() => OnInputButtonClick(action2));
                return;
        }
    }

    private void OnInputButtonClick(UnityAction action)
    {
        Field1 = input1.text;
        Field2 = input2.text;
        action.Invoke();
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

    private void CallLoginWindow(string message)
    {
        const string loginLabel = "Login";
        const string passwordLabel = "Password";
        const string registerLabel = "Register";

        ActivateAll();
        info.text = message;
        buttonLabel1.text = loginLabel;
        buttonLabel2.text = registerLabel;

        label1.text = loginLabel;
        label2.text = passwordLabel;
    }

    private void ActivateAll()
    {
        Input1.SetActive(true);
        Input2.SetActive(true);
        Button1.SetActive(true);
        Button2.SetActive(true);
    }

    private void DeactivateAll()
    {
        Input1.SetActive(false);
        Input2.SetActive(false);
        Button1.SetActive(false);
        Button2.SetActive(false);
    }
}
