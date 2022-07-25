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
    [SerializeField] protected GameObject MainWindow;

    [Header("Text Fields")]
    [SerializeField] protected TextMeshProUGUI info;
    [SerializeField] protected TextMeshProUGUI indicator;
    [SerializeField] protected TextMeshProUGUI label1;
    [SerializeField] protected TextMeshProUGUI label2;

    [Header("Components")]
    [SerializeField] protected GameObject Input1;
    [SerializeField] protected GameObject Input2;
    [SerializeField] protected GameObject ButtonParent;

    public string Field1 { get; private set; }
    public string Field2 { get; private set; }
    public static ModalWindow Instance;

    protected GameObject Button1;
    protected GameObject Button2;

    protected const string okMessage = "Ok";
    protected const string exitMessage = "Exit";

    public void CallWindow(string message, UnityAction action, string actionMessage)
    {
        DeactivateAll();
        MainWindow.SetActive(true);
        CheckButtons(1);

        info.text = message;
        DeactivateAll();
        ActivateButton(Button1, action, actionMessage);
    }

    public void CallWindow(string message, UnityAction action1, string actionMessage1,
                                           UnityAction action2, string actionMessage2)
    {
        DeactivateAll();
        MainWindow.SetActive(true);
        CheckButtons(2);

        info.text = message;
        DeactivateAll();
        ActivateButton(Button1, action1, actionMessage1);
        ActivateButton(Button2, action2, actionMessage2);
    }

    public void CallWindow(string message, UnityAction action1, string actionMessage1,
                                           UnityAction action2, string actionMessage2,
                                           string inputLabel1, string inputLabel2)
    {
        DeactivateAll();
        MainWindow.SetActive(true);
        CheckButtons(2);

        info.text = message;

        ActivateButton(Button1, action1, actionMessage1);
        ActivateButton(Button2, action2, actionMessage2);

        label1.text = inputLabel1;
        Input1.GetComponentInChildren<TMP_InputField>().onValueChanged.AddListener((value) => { Field1 = value; });
        Input1.SetActive(true);

        label2.text = inputLabel2;
        Input2.GetComponentInChildren<TMP_InputField>().onValueChanged.AddListener((value) => { Field2 = value; });
        Input2.SetActive(true);
    }

    public void SetIndicator(string indicatorMessage)
    {
        if (MainWindow.activeInHierarchy == false)
            return;

        indicator.text = indicatorMessage;
        indicator.gameObject.SetActive(true);
    }
    public void DisableButton(int buttonNum)
    {
        if (buttonNum == 1)
            Button1.GetComponent<Button>().interactable = false;
        else
            Button2.GetComponent<Button>().interactable = false;
    }
    private void ActivateButton(GameObject button, UnityAction action, string actionMessage)
    {
        button.SetActive(true);
        button.GetComponent<Button>().interactable = true;
        button.GetComponentInChildren<Text>().text = actionMessage;
        button.GetComponent<Button>().onClick.AddListener(action);
    }
    protected void CheckButtons(int count)
    {
        if (Button1 == null && count > 0)
            Button1 = Buttons.Instance.SetButton(ButtonParent, "BTN1", null);

        if (Button2 == null && count > 1)
            Button2 = Buttons.Instance.SetButton(ButtonParent, "BTN2", null);
    }

    public void CloseWindow()
    {
        MainWindow.SetActive(false);
        Button1.GetComponent<Button>().onClick.RemoveAllListeners();
        Button2.GetComponent<Button>().onClick.RemoveAllListeners();
        Input1.GetComponentInChildren<InputField>().onValueChanged.RemoveAllListeners();
        Input2.GetComponentInChildren<InputField>().onValueChanged.RemoveAllListeners();
    }

    protected void DeactivateAll()
    {
        indicator.gameObject.SetActive(false);
        if (Input1)
            Input1.SetActive(false);

        if (Input2)
            Input2.SetActive(false);

        if (Button1)
            Button1.SetActive(false);

        if (Button1)
            Button2.SetActive(false);
    }
}
