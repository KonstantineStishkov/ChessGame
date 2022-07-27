using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Inputs : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject DropListPrefab;
    [SerializeField] GameObject TogglePrefab;
    [SerializeField] GameObject LabelPrefab;

    public static Inputs Instance;
    public void Awake()
    {
        Instance = this;
    }
    public GameObject SetDropList(GameObject parent, List<string> options, UnityAction<int> action)
    {
        GameObject obj = Instantiate(DropListPrefab, parent.transform);
        TMP_Dropdown dropdown = obj.GetComponent<TMP_Dropdown>();
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(action);
        dropdown.targetGraphic = obj.GetComponent<Image>();

        return obj;
    }

    public GameObject SetToggle(GameObject parent, string label, UnityAction<bool> action)
    {
        GameObject obj = Instantiate(TogglePrefab, parent.transform);
        Toggle toggle = obj.GetComponent<Toggle>();
        obj.GetComponentInChildren<Text>().text = label;
        toggle.onValueChanged.AddListener(action);
        toggle.targetGraphic = obj.GetComponent<Image>();

        return obj;
    }
}
