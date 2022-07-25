using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject prefab;

    public static Buttons Instance;
    public void Awake()
    {
        Instance = this;
    }
    public GameObject SetButton(GameObject parent, string label, UnityAction action = null)
    {
        GameObject obj = Instantiate(prefab, parent.transform);
        obj.name = label;

        Text textComponent = obj.GetComponentInChildren<Text>();
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.text = label;

        Button btn = obj.GetComponent<Button>();

        if (action != null)
            btn.onClick.AddListener(action);

        btn.targetGraphic = obj.GetComponent<Image>();

        return obj;
    }
}
