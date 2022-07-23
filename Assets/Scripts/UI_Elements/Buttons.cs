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

    public void SetButton(GameObject parent, string label, UnityAction action)
    {
        GameObject obj = Instantiate(prefab, parent.transform);
        obj.name = label;

        Text textComponent = obj.GetComponentInChildren<Text>();
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.text = label;

        Button btn = obj.GetComponent<Button>();
        btn.onClick.AddListener(action);
        btn.targetGraphic = obj.GetComponent<Image>();
    }
}
