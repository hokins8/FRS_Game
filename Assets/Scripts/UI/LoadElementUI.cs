using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LoadElementUI : MonoBehaviour
{
    [SerializeField] TMP_Text loadName;
    [SerializeField] Button loadButton;

    public void SetupButton(string name, Action action)
    {
        loadName.text = name;
        loadButton.onClick.RemoveAllListeners();
        loadButton.onClick.AddListener(() => action.Invoke());
    }
}
