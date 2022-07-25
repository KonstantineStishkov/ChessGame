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

    [Header("Components")]
    [SerializeField] GameObject Input1;
    [SerializeField] GameObject Input2;
    [SerializeField] GameObject ButtonParent;

    public string Field1 { get; private set; }
    public string Field2 { get; private set; }

    private GameObject Button1;
    private GameObject Button2;

    const string okMessage = "Ok";
    const string exitMessage = "Exit";

    public void CallWindow(WindowType type, string message, UnityAction action1 = null, UnityAction action2 = null, string actionMessage1 = okMessage, string actionMessage2 = exitMessage)
    {
        if(Button1 == null)
            Button1 = Buttons.Instance.SetButton(ButtonParent, "BTN1", null);

        if(Button2 == null)
            Button2 = Buttons.Instance.SetButton(ButtonParent, "BTN2", null);

        MainWindow.SetActive(true);

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

    public void CloseWindow()
    {
        MainWindow.SetActive(false);
        Button1.GetComponent<Button>().onClick.RemoveAllListeners();
        Button2.GetComponent<Button>().onClick.RemoveAllListeners();
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
        Button1.GetComponentInChildren<Text>().text = okMessage;
    }

    private void CallSelectWindow(string message, string button1Message, string button2Message)
    {
        info.text = message;
        DeactivateAll();
        Button1.SetActive(true);
        Button1.GetComponentInChildren<Text>().text = button1Message;

        Button2.SetActive(true);
        Button2.GetComponentInChildren<Text>().text = button2Message;
    }

    private void CallLoginWindow(string message)
    {
        const string loginLabel = "Login";
        const string passwordLabel = "Password";
        const string registerLabel = "Register";

        ActivateAll();
        info.text = message;
        Button1.GetComponentInChildren<Text>().text = loginLabel;
        Button2.GetComponentInChildren<Text>().text = registerLabel;

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
